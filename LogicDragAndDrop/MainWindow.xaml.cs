using System;
using System.Collections.Generic;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void stack_DragOver(object sender, DragEventArgs e)
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

        private void stack_Drop(object sender, DragEventArgs e)
        {
            // If an element in the panel has already handled the drop,
            // the panel should not also handle it.
            if (e.Handled == false)
            {
                Panel _panel = (Panel)sender;
                UIElement _element = (UIElement)e.Data.GetData("Object");

                if (_panel != null && _element != null)
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

                            _panel.Children.Add(_LogicBox);
                           // _panel.ClearValue(HeightProperty);
                           // _panel.ClearValue(WidthProperty);

                            // set the value to return to the DoDragDrop call
                            e.Effects = DragDropEffects.Copy;
                            e.Handled = true;
                        }
                        else if (e.AllowedEffects.HasFlag(DragDropEffects.Move))
                        {
                            
                            if (_parent.GetType() == typeof(Border))
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
                            _panel.Children.Add(_LogicBox);
                            //_panel.ClearValue(HeightProperty);
                            //_panel.ClearValue(WidthProperty);
                            e.Effects = DragDropEffects.Move;
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        private void TryEvaluate(object sender, RoutedEventArgs e)
        {
            if(expressionStack.Children.Count != 1)
            {
                evalResult.Text = "Only one expression can be evaluated. Remove other expressions.";
            }
            else
            {
                bool? result = ((LogicBox)expressionStack.Children[0]).Evaluate();
                if(result == null)
                {
                    evalResult.Text = "Expression could not be evaluated. Ensure all values are set.";
                }else
                {
                    evalResult.Text = result.ToString();
                }
            }
        }
    }
}
