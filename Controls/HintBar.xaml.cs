using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        string text;
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
