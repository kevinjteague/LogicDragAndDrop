using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LogicDragAndDrop
{
    /// <summary>
    /// Interaction logic for LogicBox.xaml
    /// </summary>
    public partial class LogicBox : UserControl
    {
        public bool isInDragMenu{ get; set; }

        private string m_logicOperator;
        public string logicOperator
        {
            get { return m_logicOperator; }
            set
            {
                m_logicOperator = value;
                LogicOperatorChanged();
            }
                }

        private void LogicOperatorChanged()
        {
            switch (logicOperator)
            {
                case "AND":
                    logicBorder.Background = Brushes.DarkSalmon;
                    break;
                case "OR":
                    logicBorder.Background = Brushes.LightBlue;
                    break;
                case "XOR":
                    logicBorder.Background = Brushes.LightGoldenrodYellow;
                    break;
                case "NAND":
                    logicBorder.Background = Brushes.Gray;
                    break;
                case "NOR":
                    logicBorder.Background = Brushes.LightSeaGreen;
                    break;
                default:
                    break;
            }
        }

        public LogicBox()
        {
            InitializeComponent();
            logicLabel.SetBinding(ContentProperty, new Binding("logicOperator"));
            DataContext = this;
            isInDragMenu = true;
            logicOperator = "AND";
        }
        public LogicBox Copy()
        {
            LogicBox copy = new LogicBox();

            copy.isInDragMenu = false;
            copy.logicOperator = logicOperator;

            if(leftDrop.Child != null)
            {
                if (leftDrop.Child is LogicBox)
                {
                    leftDrop.ClearValue(HeightProperty);
                    leftDrop.ClearValue(WidthProperty);
                    LogicBox leftChild = (LogicBox)leftDrop.Child;
                    copy.leftDrop.Child = leftChild.Copy();
                }
            }
            if (rightDrop.Child != null)
            {
                if (rightDrop.Child is LogicBox)
                {
                    rightDrop.ClearValue(HeightProperty);
                    rightDrop.ClearValue(WidthProperty);
                    LogicBox rightChild = (LogicBox)rightDrop.Child;
                    copy.rightDrop.Child = rightChild.Copy();
                }
            }

            return copy;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // Package the data.
                DataObject data = new DataObject();
                data.SetData("Object", this);

                // Initiate the drag-and-drop operation
                if (isInDragMenu)
                {
                    DragDrop.DoDragDrop(this, data, DragDropEffects.Copy);
                }
                else
                {
                    DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
                }
                
            }
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
        {
            base.OnGiveFeedback(e);
            // These Effects values are set in the drop target's
            // DragOver event handler.
            if (e.Effects.HasFlag(DragDropEffects.Copy))
            {
                Mouse.SetCursor(Cursors.Cross);
            }
            else if (e.Effects.HasFlag(DragDropEffects.Move))
            {
                Mouse.SetCursor(Cursors.Pen);
            }
            else
            {
                Mouse.SetCursor(Cursors.No);
            }
            e.Handled = true;
        }
        private void box_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Object"))
            {
                var obj = e.Data.GetData("Object");
                if (obj.GetType() == typeof(LogicBox))
                {
                    if (((LogicBox)obj).isInDragMenu)
                    {
                        e.Effects = DragDropEffects.Copy;
                    }
                    else
                    {
                        e.Effects = DragDropEffects.Move;
                    }
                }
            }
        }

        private void box_Drop(object sender, DragEventArgs e)
        {
            // If an element in the panel has already handled the drop,
            // the panel should not also handle it.
            if (e.Handled == false)
            {
                Border _border = (Border)sender;
                UIElement _element = (UIElement)e.Data.GetData("Object");

                if (_border != null && _element != null)
                {
                    // Get the panel that the element currently belongs to,
                    // then remove it from that panel and add it the Children of
                    // the panel that its been dropped on.
                    var _parent = VisualTreeHelper.GetParent(_element);

                    if (_parent != null)
                    {
                        if (e.AllowedEffects.HasFlag(DragDropEffects.Copy))
                        {
                            LogicBox _LogicBox = ((LogicBox)_element).Copy();

                            _border.Child = _LogicBox;
                            _border.ClearValue(HeightProperty);
                            _border.ClearValue(WidthProperty);

                            // set the value to return to the DoDragDrop call
                            e.Effects = DragDropEffects.Copy;
                            e.Handled = true;
                        }else if (e.AllowedEffects.HasFlag(DragDropEffects.Move))
                        {
                            
                            if(_parent.GetType() == typeof(Border))
                            {
                                ((Border)_parent).Child = null;
                                ((Border)_parent).Height = 15;
                                ((Border)_parent).Width = 15;
                            }
                            else
                            {
                                ((Panel)_parent).Children.Remove(_element);
                            }
                            LogicBox _LogicBox = (LogicBox)_element;
                            _border.Child = _LogicBox;
                            _border.ClearValue(HeightProperty);
                            _border.ClearValue(WidthProperty);
                            e.Effects = DragDropEffects.Move;
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        private void logicBorder_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if(e.Handled == false)
            {
                if (this.Parent.GetType() == typeof(Border))
                {
                    Border parentBorder = (Border)this.Parent;
                    parentBorder.Height = 15;
                    parentBorder.Width = 15;
                    Button newButton = new Button();
                    newButton.Click += Bool_Click;
                    if(parentBorder.Name == "leftDrop")
                    {
                        newButton.Name = "leftBool";
                        parentBorder.Child = newButton;
                    }
                    else
                    {
                        newButton.Name = "rightBool";
                        parentBorder.Child = newButton;
                    }
                }
                else
                {
                    Panel parentPanel = (Panel)this.Parent;
                    parentPanel.Children.Remove(this);
                }
                e.Handled = true;
            }
        }

        private void Bool_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if(button.Content == "0")
            {
                button.Content = "1";
            }
            else
            {
                button.Content = "0";
            }
        }

        public bool? Evaluate()
        {
            bool? result;
            bool? leftSide;
            bool? rightSide;
            if(leftDrop.Child is LogicBox)
            {
                leftSide = ((LogicBox)leftDrop.Child).Evaluate();
            }else if(leftDrop.Child is Button)
            {
                leftSide = buttonBool((Button)leftDrop.Child);
            }
            else
            {
                return null;
            }

            if (rightDrop.Child is LogicBox)
            {
                rightSide = ((LogicBox)rightDrop.Child).Evaluate();
            }
            else if (rightDrop.Child is Button)
            {
                rightSide = buttonBool((Button)rightDrop.Child);
            }
            else
            {
                return null;
            }

            if (leftSide == null || rightSide == null)
            {
                return null;
            }
            else
            {
                bool left = leftSide.Value;
                bool right = rightSide.Value;
                switch (logicOperator)
                {
                    case "AND":
                        result = (left && right);
                        break;
                    case "OR":
                        result = (left || right);
                        break;
                    case "XOR":
                        result = (left != right);
                        break;
                    case "NAND":
                        result = !(left && right);
                        break;
                    case "NOR":
                        result = !(left || right);
                        break;
                    case "EQUALS":
                        result = (left == right);
                        break;
                    default:
                        result = null;
                        break;
                }
            }
            return result;
        }

        private bool? buttonBool(Button button)
        {
            switch ((string)button.Content)
            {
                case "0":
                    return false;
                case "1":
                    return true;
                default:
                    return null;
            }
        }
    }
}
