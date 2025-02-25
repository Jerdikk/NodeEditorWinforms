/*
 * The MIT License (MIT)
 *
 * Copyright (c) 2021 Mariusz Komorowski (komorra)
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES 
 * OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE 
 * OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using SampleCommon;

namespace NodeEditor
{
    public class NodesControlModel
    {
        /// <summary>
        /// Context of the editor. You should set here an instance that implements INodesContext interface.
        /// In context you should define your nodes (methods decorated by Node attribute).
        /// </summary>
        /// 
        private INodesContext context;
        public INodesContext Context
        {
            get { return context; }
            set
            {
                if (context != null)
                {
                    context.FeedbackInfo -= ContextOnFeedbackInfo;
                }
                context = value;
                if (context != null)
                {
                    context.FeedbackInfo += ContextOnFeedbackInfo;
                }
            }
        }

        public NodesGraph graph = new NodesGraph();
        public bool breakExecution = false;
        public bool rebuildConnectionDictionary = true;
        public Stack<NodeVisual> executionStack = new Stack<NodeVisual>();

        public Dictionary<string, NodeConnection> connectionDictionary = new Dictionary<string, NodeConnection>();
        /// <summary>
        /// Executes whole node graph (when called parameterless) or given node when specified.
        /// </summary>
        /// <param name="node"></param>
        public void Execute(NodeVisual node = null)
        {

            var nodeQueue = new Queue<NodeVisual>();
            nodeQueue.Enqueue(node);
            GlobalData.Instance.DeserializeContext();
            while (nodeQueue.Count > 0)
            {
                //Refresh();
                if (breakExecution)
                {
                    breakExecution = false;
                    executionStack.Clear();
                    return;
                }

                var init = nodeQueue.Dequeue() ?? graph.Nodes.FirstOrDefault(x => x.ExecInit);
                if (init != null)
                {
                    init.Feedback = FeedbackType.Debug;

                    Resolve(init);
                    init.Execute(Context);

                    if (init.OutExecPin>1)
                    {
                        int yy = 1;
                    }

                    NodeConnection connection = null;

                    /*foreach (NodeConnection nodeConnection in graph.Connections)
                    {
                        if (nodeConnection.OutputNode == init)
                        {
                            if (nodeConnection.IsExecution)
                            {
                                if (nodeConnection.OutputSocket.Value != null)
                                {
                                    if ((nodeConnection.OutputSocket.Value as ExecutionPath).IsSignaled)
                                    {
                                        connection = nodeConnection;
                                        break;
                                    }
                                    else
                                    {

                                    }
                                }
                                else
                                {

                                }
                            }
                            else
                            {

                            }
                        }
                        else
                        {

                        }
                    }*/
                    // эта штука никогда не сработает, нужно понять что такое сигнал...
                    connection = graph.Connections.FirstOrDefault( x => x.OutputNode == init && x.IsExecution && x.OutputSocket.Value != null && (x.OutputSocket.Value as ExecutionPath).IsSignaled);

                    if (connection == null)
                    {
                        connection = graph.Connections.FirstOrDefault(x => x.OutputNode == init && x.IsExecution && x.OutputSocket.IsMainExecution);
                    }
                    else
                    {
                        executionStack.Push(init);
                    }
                    if (connection != null)
                    {
                        connection.InputNode.IsBackExecuted = false;
                        //Execute(connection.InputNode);
                        nodeQueue.Enqueue(connection.InputNode);
                    }
                    else
                    {
                        if (executionStack.Count > 0)
                        {
                            var back = executionStack.Pop();
                            back.IsBackExecuted = true;
                            Execute(back);
                        }
                    }
                }
            }
            //  GlobalData.Instance.globalContext.ProgrammCounter++;
            GlobalData.Instance.SerializeContext();
        }

        public bool IsConnectable(SocketVisual a, SocketVisual b)
        {
            var input = a.Input ? a : b;
            var output = a.Input ? b : a;
            var otype = Type.GetType(output.Type.FullName.Replace("&", ""), AssemblyResolver, TypeResolver);
            var itype = Type.GetType(input.Type.FullName.Replace("&", ""), AssemblyResolver, TypeResolver);
            if (otype == null || itype == null) return false;
            var allow = otype == itype || otype.IsSubclassOf(itype);
            return allow;
        }

        public Type TypeResolver(Assembly assembly, string name, bool inh)
        {
            if (assembly == null) assembly = ResolveAssembly(name);
            if (assembly == null) return null;
            return assembly.GetType(name);
        }

        public Assembly ResolveAssembly(string fullTypeName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(x => x.GetTypes().Any(o => o.FullName == fullTypeName));
        }
        public Assembly AssemblyResolver(AssemblyName assemblyName)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.FullName == assemblyName.FullName);
        }
        public void ExecuteResolving(params string[] nodeNames)
        {
            var nodes = graph.Nodes.Where(x => nodeNames.Contains(x.Name));

            foreach (var node in nodes)
            {
                ExecuteResolvingInternal(node);
            }
        }

        private void ExecuteResolvingInternal(NodeVisual node)
        {
            var icontext = (node.GetNodeContext() as DynamicNodeContext);
            foreach (var input in node.GetInputs(false))
            {
                var connection =
                    graph.Connections.FirstOrDefault(x => x.InputNode == node && x.InputSocketName == input.Name);
                if (connection != null)
                {
                    Resolve(connection.OutputNode);

                    connection.OutputNode.Execute(Context);

                    ExecuteResolvingInternal(connection.OutputNode);

                    var ocontext = (connection.OutputNode.GetNodeContext() as DynamicNodeContext);
                    icontext[connection.InputSocketName] = ocontext[connection.OutputSocketName];
                }
            }
        }

        /// <summary>
        /// Resolves given node, resolving it all dependencies recursively.
        /// </summary>
        /// <param name="node"></param>
        private void Resolve(NodeVisual node)
        {
            DynamicNodeContext icontext = (node.GetNodeContext() as DynamicNodeContext);
            foreach (var input in node.GetInputs(false))
            {
                var connection = GetConnection(node.GUID + input.Name);
                //graph.Connections.FirstOrDefault(x => x.InputNode == node && x.InputSocketName == input.Name);
                if (connection != null)
                {
                    Resolve(connection.OutputNode);
                    if (!connection.OutputNode.Callable)
                    {
                        connection.OutputNode.Execute(Context);
                    }
                    DynamicNodeContext ocontext = (connection.OutputNode.GetNodeContext() as DynamicNodeContext);
                    icontext[connection.InputSocketName] = ocontext[connection.OutputSocketName];
                }
            }
        }

        private NodeConnection GetConnection(string v)
        {
            if (rebuildConnectionDictionary)
            {
                rebuildConnectionDictionary = false;
                connectionDictionary.Clear();
                foreach (var conn in graph.Connections)
                {
                    connectionDictionary.Add(conn.InputNode.GUID + conn.InputSocketName, conn);
                }
            }
            NodeConnection nc = null;
            if (connectionDictionary.TryGetValue(v, out nc))
            {
                return nc;
            }
            return null;
        }

        private void ContextOnFeedbackInfo(string message, NodeVisual nodeVisual, FeedbackType type, object tag, bool breakExecution)
        {
            this.breakExecution = breakExecution;
            if (breakExecution)
            {
                nodeVisual.Feedback = type;
                //  OnNodeHint(message);
            }
        }

        /// <summary>
        /// Serializes current node graph to binary data.
        /// </summary>        
        public byte[] Serialize()
        {
            try
            {
                using (var bw = new BinaryWriter(new MemoryStream()))
                {
                    bw.Write("NodeSystemP"); //recognization string
                    bw.Write(1000); //version
                    bw.Write(graph.Nodes.Count);
                    foreach (var node in graph.Nodes)
                    {
                        SerializeNode(bw, node);
                    }
                    bw.Write(graph.Connections.Count);
                    foreach (var connection in graph.Connections)
                    {
                        bw.Write(connection.OutputNode.GUID);
                        bw.Write(connection.OutputSocketName);

                        bw.Write(connection.InputNode.GUID);
                        bw.Write(connection.InputSocketName);
                        bw.Write(0); //additional data size per connection
                    }
                    bw.Write(0); //additional data size per graph
                    return (bw.BaseStream as MemoryStream).ToArray();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public void SerializeNode(BinaryWriter bw, NodeVisual node)
        {
            try
            {
                bw.Write(node.GUID);
                bw.Write(node.X);
                bw.Write(node.Y);
                bw.Write(node.Callable);
                bw.Write(node.IsOnlyOut);
                bw.Write(node.OutExecPin);
                bw.Write(node.ExecInit);
                bw.Write(node.Name);
                bw.Write(node.Order);
                if (node.CustomEditor == null)
                {
                    bw.Write("");
                    bw.Write("");
                }
                else
                {
                    bw.Write(node.CustomEditor.GetType().Assembly.GetName().Name);
                    bw.Write(node.CustomEditor.GetType().FullName);
                }
                bw.Write(node.Type.Name);
                var context = (node.GetNodeContext() as DynamicNodeContext).Serialize();
                bw.Write(context.Length);
                bw.Write(context);
                bw.Write(8); //additional data size per node
                bw.Write(node.Int32Tag);
                bw.Write(node.NodeColor.ToArgb());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Restores node graph state from previously serialized binary data.
        /// </summary>
        /// <param name="data"></param>
        public void Deserialize(byte[] data)
        {
            try
            {
                using (var br = new BinaryReader(new MemoryStream(data)))
                {
                    var ident = br.ReadString();
                    if (ident != "NodeSystemP") return;
                    rebuildConnectionDictionary = true;
                    graph.Connections.Clear();
                    graph.Nodes.Clear();
                    //  Controls.Clear();

                    var version = br.ReadInt32();
                    int nodeCount = br.ReadInt32();
                    for (int i = 0; i < nodeCount; i++)
                    {
                        var nv = DeserializeNode(br);

                        graph.Nodes.Add(nv);
                    }
                    var connectionsCount = br.ReadInt32();
                    for (int i = 0; i < connectionsCount; i++)
                    {
                        var con = new NodeConnection();
                        var og = br.ReadString();
                        con.OutputNode = graph.Nodes.FirstOrDefault(x => x.GUID == og);
                        con.OutputSocketName = br.ReadString();
                        var ig = br.ReadString();
                        con.InputNode = graph.Nodes.FirstOrDefault(x => x.GUID == ig);
                        con.InputSocketName = br.ReadString();
                        br.ReadBytes(br.ReadInt32()); //read additional data

                        graph.Connections.Add(con);
                        rebuildConnectionDictionary = true;
                    }
                    br.ReadBytes(br.ReadInt32()); //read additional data
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public NodeVisual DeserializeNode(BinaryReader br)
        {
            var nv = new NodeVisual();
            try
            {

                nv.GUID = br.ReadString();
                nv.X = br.ReadSingle();
                nv.Y = br.ReadSingle();
                nv.Callable = br.ReadBoolean();
                nv.IsOnlyOut = br.ReadBoolean();
                nv.OutExecPin = br.ReadInt32();
                nv.ExecInit = br.ReadBoolean();
                nv.Name = br.ReadString();
                nv.Order = br.ReadInt32();
                var customEditorAssembly = br.ReadString();
                var customEditor = br.ReadString();
                nv.Type = Context.GetType().GetMethod(br.ReadString());
                var attribute = nv.Type.GetCustomAttributes(typeof(NodeAttribute), false)
                                            .Cast<NodeAttribute>()
                                            .FirstOrDefault();
                if (attribute != null)
                {
                    nv.CustomWidth = attribute.Width;
                    nv.CustomHeight = attribute.Height;
                }
            (nv.GetNodeContext() as DynamicNodeContext).Deserialize(br.ReadBytes(br.ReadInt32()));
                var additional = br.ReadInt32(); //read additional data
                if (additional >= 4)
                {
                    nv.Int32Tag = br.ReadInt32();
                    if (additional >= 8)
                    {
                        nv.NodeColor = Color.FromArgb(br.ReadInt32());
                    }
                }
                if (additional > 8)
                {
                    br.ReadBytes(additional - 8);
                }

                /*if (customEditor != "")
                {
                    nv.CustomEditor =
                        Activator.CreateInstance(AppDomain.CurrentDomain, customEditorAssembly, customEditor).Unwrap() as Control;

                    Control ctrl = nv.CustomEditor;
                    if (ctrl != null)
                    {
                        ctrl.Tag = nv;
                        Controls.Add(ctrl);
                    }
                    nv.LayoutEditor();
                }*/

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return nv;
        }

        /// <summary>
        /// Clears node graph state.
        /// </summary>
        public void Clear()
        {
            graph.Nodes.Clear();
            graph.Connections.Clear();
            rebuildConnectionDictionary = true;
        }

        public List<NodeToken> GetNodeTokensList()
        {
            if (Context == null) return null;
            var methods = Context.GetType().GetMethods();
            List<NodeToken> nodes =
                methods.Select(
                    x =>
                        new
                            NodeToken()
                        {
                            Method = x,
                            Attribute =
                                x.GetCustomAttributes(typeof(NodeAttribute), false)
                                    .Cast<NodeAttribute>()
                                    .FirstOrDefault()
                        }).Where(x => x.Attribute != null).ToList<NodeToken>();

            return nodes;
        }

    }
}
