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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TreeView;
using System.Runtime.Remoting.Contexts;

namespace NodeEditor
{

    /// <summary>
    /// Main control of Node Editor Winforms
    /// </summary>
    [ToolboxBitmap(typeof(NodesControl), "nodeed")]
    public partial class NodesControl : UserControl
    {

        public NodesControlModel model = new NodesControlModel();

        
        private bool needRepaint = true;
        private Timer timer = new Timer();
        private bool mdown;
        private Point lastmpos;
        private SocketVisual dragSocket;
        private NodeVisual dragSocketNode;
        private PointF dragConnectionBegin;
        private PointF dragConnectionEnd;     

   
        /// <summary>
        /// Occurs when user selects a node. In the object will be passed node settings for unplugged inputs/outputs.
        /// </summary>
        public event Action<object> OnNodeContextSelected = delegate { };

        /// <summary>
        /// Occurs when node would to share its description.
        /// </summary>
        public event Action<string> OnNodeHint = delegate { };

        /// <summary>
        /// Indicates which part of control should be actually visible. It is useful when dragging nodes out of autoscroll parent control,
        /// to guarantee that moving node/connection is visible to user.
        /// </summary>
        public event Action<RectangleF> OnShowLocation = delegate { };

        private readonly Dictionary<ToolStripMenuItem, int> allContextItems = new Dictionary<ToolStripMenuItem, int>();

        private Point lastMouseLocation;

        private Point autoScroll;

        private PointF selectionStart;

        private PointF selectionEnd;



        /// <summary>
        /// Default constructor
        /// </summary>
        public NodesControl()
        {
            InitializeComponent();
            timer.Interval = 30;
            timer.Tick += TimerOnTick;
            timer.Start();
            KeyDown += OnKeyDown;
            SetStyle(ControlStyles.Selectable, true);
        }


        /// <summary>
        /// Clears node graph state.
        /// </summary>
        public void Clear()
        {
            //model.Clear();
            Controls.Clear();
            Refresh();
        }
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 7)
            {
                return;
            }
            base.WndProc(ref m);
        }

        private void OnKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            if (keyEventArgs.KeyCode == Keys.Delete)
            {
                DeleteSelectedNodes();
            }
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            if (DesignMode) return;
            if (needRepaint)
            {
                Invalidate();
            }
        }

        private void NodesControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBilinear;

            model.graph.Draw(e.Graphics, PointToClient(MousePosition), MouseButtons);

            if (dragSocket != null)
            {
                var pen = new Pen(Color.Black, 2);
                NodesGraph.DrawConnection(e.Graphics, pen, dragConnectionBegin, dragConnectionEnd);
            }

            if (selectionStart != PointF.Empty)
            {
                var rect = Rectangle.Round(MakeRect(selectionStart, selectionEnd));
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(50, Color.CornflowerBlue)), rect);
                e.Graphics.DrawRectangle(new Pen(Color.DodgerBlue), rect);
            }

            needRepaint = false;
        }

        private static RectangleF MakeRect(PointF a, PointF b)
        {
            var x1 = a.X;
            var x2 = b.X;
            var y1 = a.Y;
            var y2 = b.Y;
            return new RectangleF(Math.Min(x1, x2), Math.Min(y1, y2), Math.Abs(x2 - x1), Math.Abs(y2 - y1));
        }

        private void NodesControl_MouseMove(object sender, MouseEventArgs e)
        {
            var em = PointToScreen(e.Location);
            if (selectionStart != PointF.Empty)
            {
                selectionEnd = e.Location;
            }
            if (mdown)
            {
                foreach (var node in model.graph.Nodes.Where(x => x.IsSelected))
                {
                    node.X += em.X - lastmpos.X;
                    node.Y += em.Y - lastmpos.Y;
                    node.DiscardCache();
                    node.LayoutEditor();
                }
                if (model.graph.Nodes.Exists(x => x.IsSelected))
                {
                    var n = model.graph.Nodes.FirstOrDefault(x => x.IsSelected);
                    var bound = new RectangleF(new PointF(n.X, n.Y), n.GetNodeBounds());
                    foreach (var node in model.graph.Nodes.Where(x => x.IsSelected))
                    {
                        bound = RectangleF.Union(bound, new RectangleF(new PointF(node.X, node.Y), node.GetNodeBounds()));
                    }
                    OnShowLocation(bound);
                }
                Invalidate();

                if (dragSocket != null)
                {
                    var center = new PointF(dragSocket.X + dragSocket.Width / 2f, dragSocket.Y + dragSocket.Height / 2f);
                    if (dragSocket.Input)
                    {
                        dragConnectionBegin.X += em.X - lastmpos.X;
                        dragConnectionBegin.Y += em.Y - lastmpos.Y;
                        dragConnectionEnd = center;
                        OnShowLocation(new RectangleF(dragConnectionBegin, new SizeF(10, 10)));
                    }
                    else
                    {
                        dragConnectionBegin = center;
                        dragConnectionEnd.X += em.X - lastmpos.X;
                        dragConnectionEnd.Y += em.Y - lastmpos.Y;
                        OnShowLocation(new RectangleF(dragConnectionEnd, new SizeF(10, 10)));
                    }

                }
                lastmpos = em;
            }

            needRepaint = true;
        }

        private void NodesControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectionStart = PointF.Empty;

                Focus();

                if ((ModifierKeys & Keys.Shift) != Keys.Shift)
                {
                    model.graph.Nodes.ForEach(x => x.IsSelected = false);
                }

                var node =
                    model.graph.Nodes.OrderBy(x => x.Order).FirstOrDefault(
                        x => new RectangleF(new PointF(x.X, x.Y), x.GetHeaderSize()).Contains(e.Location));

                if (node != null && !mdown)
                {

                    node.IsSelected = true;

                    node.Order = model.graph.Nodes.Min(x => x.Order) - 1;
                    if (node.CustomEditor != null)
                    {
                        node.CustomEditor.BringToFront();
                    }
                    mdown = true;
                    lastmpos = PointToScreen(e.Location);

                    Refresh();
                }
                if (node == null && !mdown)
                {
                    var nodeWhole =
                    model.graph.Nodes.OrderBy(x => x.Order).FirstOrDefault(
                        x => new RectangleF(new PointF(x.X, x.Y), x.GetNodeBounds()).Contains(e.Location));
                    if (nodeWhole != null)
                    {
                        node = nodeWhole;
                        var socket = nodeWhole.GetSockets().FirstOrDefault(x => x.GetBounds().Contains(e.Location));
                        if (socket != null)
                        {
                            if ((ModifierKeys & Keys.Control) == Keys.Control)
                            {
                                var connection =
                                    model.graph.Connections.FirstOrDefault(
                                        x => x.InputNode == nodeWhole && x.InputSocketName == socket.Name);

                                if (connection != null)
                                {
                                    dragSocket =
                                        connection.OutputNode.GetSockets()
                                            .FirstOrDefault(x => x.Name == connection.OutputSocketName);
                                    dragSocketNode = connection.OutputNode;
                                }
                                else
                                {
                                    connection =
                                        model.graph.Connections.FirstOrDefault(
                                            x => x.OutputNode == nodeWhole && x.OutputSocketName == socket.Name);

                                    if (connection != null)
                                    {
                                        dragSocket =
                                            connection.InputNode.GetSockets()
                                                .FirstOrDefault(x => x.Name == connection.InputSocketName);
                                        dragSocketNode = connection.InputNode;
                                    }
                                }

                                model.graph.Connections.Remove(connection);
                                model.rebuildConnectionDictionary = true;
                            }
                            else
                            {
                                dragSocket = socket;
                                dragSocketNode = nodeWhole;
                            }
                            dragConnectionBegin = e.Location;
                            dragConnectionEnd = e.Location;
                            mdown = true;
                            lastmpos = PointToScreen(e.Location);
                        }
                    }
                    else
                    {
                        selectionStart = selectionEnd = e.Location;
                    }
                }
                if (node != null)
                {
                    OnNodeContextSelected(node.GetNodeContext());
                }
            }

            needRepaint = true;
        }

     
        private void NodesControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (selectionStart != PointF.Empty)
            {
                var rect = MakeRect(selectionStart, selectionEnd);
                model.graph.Nodes.ForEach(
                    x => x.IsSelected = rect.Contains(new RectangleF(new PointF(x.X, x.Y), x.GetNodeBounds())));
                selectionStart = PointF.Empty;
            }

            if (dragSocket != null)
            {
                var nodeWhole =
                    model.graph.Nodes.OrderBy(x => x.Order).FirstOrDefault(
                        x => new RectangleF(new PointF(x.X, x.Y), x.GetNodeBounds()).Contains(e.Location));
                if (nodeWhole != null)
                {
                    var socket = nodeWhole.GetSockets().FirstOrDefault(x => x.GetBounds().Contains(e.Location));
                    if (socket != null)
                    {
                        if (model.IsConnectable(dragSocket, socket) && dragSocket.Input != socket.Input)
                        {
                            var nc = new NodeConnection();
                            if (!dragSocket.Input)
                            {
                                nc.OutputNode = dragSocketNode;
                                nc.OutputSocketName = dragSocket.Name;
                                nc.InputNode = nodeWhole;
                                nc.InputSocketName = socket.Name;
                            }
                            else
                            {
                                nc.InputNode = dragSocketNode;
                                nc.InputSocketName = dragSocket.Name;
                                nc.OutputNode = nodeWhole;
                                nc.OutputSocketName = socket.Name;
                            }

                            model.graph.Connections.RemoveAll(
                                x => x.InputNode == nc.InputNode && x.InputSocketName == nc.InputSocketName);

                            model.graph.Connections.Add(nc);
                            model.rebuildConnectionDictionary = true;
                        }
                    }
                }
            }

            dragSocket = null;
            mdown = false;
            needRepaint = true;
        }
        private void AddToMenu(ToolStripItemCollection items, NodeToken token, string path, EventHandler click)
        {
            var pathParts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var first = pathParts.FirstOrDefault();
            ToolStripMenuItem item = null;
            if (!items.ContainsKey(first))
            {
                item = new ToolStripMenuItem(first);
                item.Name = first;
                item.Tag = token;
                items.Add(item);
            }
            else
            {
                item = items[first] as ToolStripMenuItem;
            }
            var next = string.Join("/", pathParts.Skip(1));
            if (!string.IsNullOrEmpty(next))
            {
                item.MouseEnter += (sender, args) => OnNodeHint("");
                AddToMenu(item.DropDownItems, token, next, click);
            }
            else
            {
                item.Click += click;
                item.Click += (sender, args) =>
                {
                    var i = allContextItems.Keys.FirstOrDefault(x => x.Name == item.Name);
                    allContextItems[i]++;
                };
                item.MouseEnter += (sender, args) => OnNodeHint(token.Attribute.Description ?? "");
                if (!allContextItems.Keys.Any(x => x.Name == item.Name))
                {
                    allContextItems.Add(item, 0);
                }
            }
        }

        private void NodesControl_MouseClick(object sender, MouseEventArgs e)
        {
            lastMouseLocation = e.Location;

           // if (Context == null) return;

            if (e.Button == MouseButtons.Right)
            {
               

                var context = new ContextMenuStrip();
                if (model.graph.Nodes.Exists(x => x.IsSelected))
                {
                    context.Items.Add("Delete Node(s)", null, ((o, args) =>
                    {
                        DeleteSelectedNodes();
                    }));
                    context.Items.Add("Duplicate Node(s)", null, ((o, args) =>
                    {
                        DuplicateSelectedNodes();
                    }));
                    context.Items.Add("Change Color ...", null, ((o, args) =>
                    {
                        ChangeSelectedNodesColor();
                    }));
                    if (model.graph.Nodes.Count(x => x.IsSelected) == 2)
                    {
                        var sel = model.graph.Nodes.Where(x => x.IsSelected).ToArray();
                        context.Items.Add("Check Impact", null, ((o, args) =>
                        {
                            if (HasImpact(sel[0], sel[1]) || HasImpact(sel[1], sel[0]))
                            {
                                MessageBox.Show("One node has impact on other.", "Impact detected.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("These nodes not impacts themselves.", "No impact.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }));
                    }
                    context.Items.Add(new ToolStripSeparator());
                }
                if (allContextItems.Values.Any(x => x > 0))
                {
                    var handy = allContextItems.Where(x => x.Value > 0 && !string.IsNullOrEmpty(((x.Key.Tag) as NodeToken).Attribute.Menu)).OrderByDescending(x => x.Value).Take(8);
                    foreach (var kv in handy)
                    {
                        context.Items.Add(kv.Key);
                    }
                    context.Items.Add(new ToolStripSeparator());
                }

                var nodes = model.GetNodeTokensList();

                foreach (var node in nodes.OrderBy(x => x.Attribute.Path))
                {
                    AddToMenu(context.Items, node, node.Attribute.Path, (s, ev) =>
                    {
                        var tag = (s as ToolStripMenuItem).Tag as NodeToken;

                        var nv = new NodeVisual();
                        nv.X = lastMouseLocation.X;
                        nv.Y = lastMouseLocation.Y;
                        nv.Type = node.Method;
                        nv.Callable = node.Attribute.IsCallable;
                        nv.IsOnlyOut = node.Attribute.IsOnlyOut;
                        nv.OutExecPin = node.Attribute.OutExec;
                        nv.Name = node.Attribute.Name;
                        nv.Order = model.graph.Nodes.Count;
                        nv.ExecInit = node.Attribute.IsExecutionInitiator;
                        nv.XmlExportName = node.Attribute.XmlExportName;
                        nv.CustomWidth = node.Attribute.Width;
                        nv.CustomHeight = node.Attribute.Height;

                        if (node.Attribute.CustomEditor != null)
                        {
                            Control ctrl = null;
                            nv.CustomEditor = ctrl = Activator.CreateInstance(node.Attribute.CustomEditor) as Control;
                            if (ctrl != null)
                            {
                                ctrl.Tag = nv;
                                Controls.Add(ctrl);
                            }
                            nv.LayoutEditor();
                        }

                        model.graph.Nodes.Add(nv);
                        Refresh();
                        needRepaint = true;
                    });
                }
                context.Show(MousePosition);
            }
        }

        private void ChangeSelectedNodesColor()
        {
            ColorDialog cd = new ColorDialog();
            cd.FullOpen = true;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                foreach (var n in model.graph.Nodes.Where(x => x.IsSelected))
                {
                    n.NodeColor = cd.Color;
                }
            }
            Refresh();
            needRepaint = true;
        }

        private void DuplicateSelectedNodes()
        {
            var cloned = new List<NodeVisual>();
            foreach (var n in model.graph.Nodes.Where(x => x.IsSelected))
            {
                int count = model.graph.Nodes.Count(x => x.IsSelected);
                var ms = new MemoryStream();
                var bw = new BinaryWriter(ms);
                model.SerializeNode(bw, n);
                ms.Seek(0, SeekOrigin.Begin);
                var br = new BinaryReader(ms);
                var clone = model.DeserializeNode(br);
                clone.X += 40;
                clone.Y += 40;
                clone.GUID = Guid.NewGuid().ToString();
                cloned.Add(clone);
                br.Dispose();
                bw.Dispose();
                ms.Dispose();
            }
            model.graph.Nodes.ForEach(x => x.IsSelected = false);
            cloned.ForEach(x => x.IsSelected = true);
            cloned.Where(x => x.CustomEditor != null).ToList().ForEach(x => x.CustomEditor.BringToFront());
            model.graph.Nodes.AddRange(cloned);
            Invalidate();
        }

        private void DeleteSelectedNodes()
        {
            if (model.graph.Nodes.Exists(x => x.IsSelected))
            {
                foreach (var n in model.graph.Nodes.Where(x => x.IsSelected))
                {
                    Controls.Remove(n.CustomEditor);
                    model.graph.Connections.RemoveAll(
                        x => x.OutputNode == n || x.InputNode == n);
                }
                model.graph.Nodes.RemoveAll(x => model.graph.Nodes.Where(n => n.IsSelected).Contains(x));
            }
            Invalidate();
        }

  
        public List<NodeVisual> GetNodes(params string[] nodeNames)
        {
            var nodes = model.graph.Nodes.Where(x => nodeNames.Contains(x.Name));
            return nodes.ToList();
        }

        public bool HasImpact(NodeVisual startNode, NodeVisual endNode)
        {
            var connections = model.graph.Connections.Where(x => x.OutputNode == startNode && !x.IsExecution);
            foreach (var connection in connections)
            {
                if (connection.InputNode == endNode)
                {
                    return true;
                }
                bool nextImpact = HasImpact(connection.InputNode, endNode);
                if (nextImpact)
                {
                    return true;
                }
            }

            return false;
        }


        public string ExportToXml()
        {
            var xml = new XmlDocument();

            XmlElement el = (XmlElement)xml.AppendChild(xml.CreateElement("NodeGrap"));
            el.SetAttribute("Created", DateTime.Now.ToString());
            var nodes = el.AppendChild(xml.CreateElement("Nodes"));
            foreach (var node in model.graph.Nodes)
            {
                var xmlNode = (XmlElement)nodes.AppendChild(xml.CreateElement("Node"));
                xmlNode.SetAttribute("Name", node.XmlExportName);
                xmlNode.SetAttribute("Id", node.GetGuid());
                var xmlContext = (XmlElement)xmlNode.AppendChild(xml.CreateElement("Context"));
                var context = node.GetNodeContext() as DynamicNodeContext;
                foreach (var kv in context)
                {
                    var ce = (XmlElement)xmlContext.AppendChild(xml.CreateElement("ContextMember"));
                    ce.SetAttribute("Name", kv);
                    ce.SetAttribute("Value", Convert.ToString(context[kv] ?? ""));
                    ce.SetAttribute("Type", context[kv] == null ? "" : context[kv].GetType().FullName);
                }
            }
            var connections = el.AppendChild(xml.CreateElement("Connections"));
            foreach (var conn in model.graph.Connections)
            {
                var xmlConn = (XmlElement)connections.AppendChild(xml.CreateElement("Connection"));
                xmlConn.SetAttribute("OutputNodeId", conn.OutputNode.GetGuid());
                xmlConn.SetAttribute("OutputNodeSocket", conn.OutputSocketName);
                xmlConn.SetAttribute("InputNodeId", conn.InputNode.GetGuid());
                xmlConn.SetAttribute("InputNodeSocket", conn.InputSocketName);
            }
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                xml.Save(writer);
            }
            return sb.ToString();
        }

  
       }
}
