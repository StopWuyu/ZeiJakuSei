using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using ZeiJakuSei.Modules;

namespace ZeiJakuSei.Controls
{
    /// <summary>
    /// HintBar.xaml 的交互逻辑
    /// </summary>
    public partial class HintBar : Border
    {
        public HintBar()
        {
            InitializeComponent();
            Opacity = 0;
        }

        bool isLeave = false;

        #region 属性

        // 新建属性 Text
        string text = "Text";
        public static readonly DependencyProperty textProperty =
            DependencyProperty.Register(
                "Text",
                typeof(string),
                typeof(HintBar),
                new PropertyMetadata("Text"));
        public string Text
        {
            set
            {
                text = value;
                HintText.Text = value;
            }

            get
            {
                return text;
            }
        }

        #endregion

        #region 方法

        public void onLoad()
        {
            isLeave = false;
            ModAnimation.Animate(this, OpacityProperty, 1, TimeSpan.FromMilliseconds(300));

            ModBase.RunInNewThread(() =>
            {
                Thread.Sleep(5000);
                ModBase.RunInUI(() => 
                {
                    onLeave();
                });
            });
        }

        public void onLeave()
        {
            if (isLeave)
            {
                return;
            }
            isLeave = true;
            Opacity = 1;
            ModAnimation.Animate(this, OpacityProperty, 0, TimeSpan.FromMilliseconds(300));
        }

        #endregion
    }
}
