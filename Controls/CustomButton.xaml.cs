using ZeiJakuSei.Modules;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ZeiJakuSei.Controls
{
    /// <summary>
    /// CustomButton.xaml 的交互逻辑
    /// </summary>
    public partial class CustomButton : Border
    {
        public CustomButton()
        {
            InitializeComponent();

            text ??= "Button";
        }
        private void CustomButton_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonScale.CenterX = ButtonBorder.ActualWidth / 2;
            ButtonScale.CenterY = ButtonBorder.ActualHeight / 2;
        }

        #region 属性

        bool isClick = false;

        // 新建属性 Text
        string text;
        public static readonly DependencyProperty textProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(CustomButton),
                new PropertyMetadata("Button"));
        public string Text
        {
            set
            {
                text = value;
                ButtonText.Text = value;
            }

            get
            {
                return text;
            }
        }
        // 新建属性 FontSize
        int fontSize;
        public static readonly DependencyProperty fontSizeProperty =
            DependencyProperty.Register(
                "FontSize",
                typeof(int),
                typeof(CustomButton),
                new PropertyMetadata(18));
        public int FontSize
        {
            set
            {
                fontSize = value;
                ButtonText.FontSize = value;
            }

            get
            {
                return fontSize;
            }
        }

        #endregion

        #region 事件

        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Border));
        public event RoutedEventHandler Click
        {
            add
            {
                AddHandler(ClickEvent, value);
            }
            remove
            {
                RemoveHandler(ClickEvent, value);
            }
        }

        #endregion

        #region 方法

        private void Button_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            isClick = true;
            ModAnimation.AnimateScale(this, ScaleTransform.ScaleXProperty, 0.95, TimeSpan.FromMilliseconds(100));
            ModAnimation.AnimateScale(this, ScaleTransform.ScaleYProperty, 0.95, TimeSpan.FromMilliseconds(100));
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            ModAnimation.Animate(this, BackgroundProperty, Colors.LightGray, TimeSpan.FromMilliseconds(100));
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            e.Handled = true;

            isClick = false;
            ModAnimation.Animate(this, BackgroundProperty, Colors.Transparent, TimeSpan.FromMilliseconds(100));
            ModAnimation.AnimateScale(this, ScaleTransform.ScaleXProperty, 1, TimeSpan.FromMilliseconds(100));
            ModAnimation.AnimateScale(this, ScaleTransform.ScaleYProperty, 1, TimeSpan.FromMilliseconds(100));
        }

        private void Button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;

            ModAnimation.AnimateScale(this, ScaleTransform.ScaleXProperty, 1, TimeSpan.FromMilliseconds(100));
            ModAnimation.AnimateScale(this, ScaleTransform.ScaleYProperty, 1, TimeSpan.FromMilliseconds(100));

            if (isClick)
            {
                RaiseEvent(new RoutedEventArgs(ClickEvent, this));
            }
        }

        #endregion
    }
}
