using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace _2048_UWP
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        int[,] num;
        Tile[,] tiles;
        readonly static int LEFT = 0;
        readonly static int UP = 1;
        readonly static int RIGHT = 2;
        readonly static int DOWN = 3;

        private int times = 0;
        Point start;

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
            bigmain.ManipulationStarted += Main_ManipulationStarted;
            bigmain.ManipulationDelta += Main_ManipulationDelta;
            bigmain.ManipulationCompleted += Main_ManipulationCompleted;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            #region
            tiles = new Tile[4, 4];
            tiles[0, 0] = b00;
            tiles[0, 1] = b01;
            tiles[0, 2] = b02;
            tiles[0, 3] = b03;

            tiles[1, 0] = b10;
            tiles[1, 1] = b11;
            tiles[1, 2] = b12;
            tiles[1, 3] = b13;

            tiles[2, 0] = b20;
            tiles[2, 1] = b21;
            tiles[2, 2] = b22;
            tiles[2, 3] = b23;

            tiles[3, 0] = b30;
            tiles[3, 1] = b31;
            tiles[3, 2] = b32;
            tiles[3, 3] = b33;
            #endregion
            btn.Focus(FocusState.Programmatic);
            NewGame(times++);
        }

        private void Page_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            Debug.WriteLine("key: " + e.Key);
            if (e.Key == Windows.System.VirtualKey.Left || e.Key == Windows.System.VirtualKey.A || e.Key == Windows.System.VirtualKey.H)
            {
                Move(LEFT);
            }
            if (e.Key == Windows.System.VirtualKey.Right || e.Key == Windows.System.VirtualKey.D || e.Key == Windows.System.VirtualKey.L)
            {
                Move(RIGHT);
            }
            if (e.Key == Windows.System.VirtualKey.Up || e.Key == Windows.System.VirtualKey.W || e.Key == Windows.System.VirtualKey.K)
            {
                Move(UP);
            }
            if (e.Key == Windows.System.VirtualKey.Down || e.Key == Windows.System.VirtualKey.S || e.Key == Windows.System.VirtualKey.J)
            {
                Move(DOWN);
            }
            IfGameOver();

        }

        private void Main_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            start = e.Position;
        }

        private void Main_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            Point end = e.Position;
            e.Complete();
            if (Math.Abs(end.X - start.X) < 5 && Math.Abs(end.Y - start.Y) < 5)
            {
                return;
            }
            if (Math.Abs(end.X - start.X) > Math.Abs(end.Y - start.Y))
            {
                if (end.X - start.X > 0) { Move(RIGHT); }
                else { Move(LEFT); }
            }
            else
            {
                if (end.Y - start.Y > 0) { Move(DOWN); }
                else { Move(UP); }
            }
            IfGameOver();
        }

        private void Main_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e) { }

        private void Move(int o)
        {
            // 初始化num中间数组为全零
            num = new int[4, 4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    num[i, j] = 0;
                }
            }

            //清除没有数字的格子
            bool hasBlankMove = ClearBlank(o);

            /** 记录得分 */
            int i_score = int.Parse(score.Text);

            bool hasAddMove = AddNumber(o, ref i_score);

            score.Text = i_score.ToString();
            if (i_score > int.Parse(best.Text)) { best.Text = i_score.ToString(); }

            /** 产生新的数字块 */
            if (hasAddMove | hasBlankMove)
            {
                // 产生1个新的数字
                Random random = new Random();
                int a = random.Next(15);
                if (a == 0)
                    a = 4;
                else
                    a = 2;
                int x = 0, y = 0;
                do
                {   // 产生[0,3]随机数
                    x = random.Next(4);
                    y = random.Next(4);
                } while (tiles[x, y].Number != 0);
                tiles[x, y].Number = a;
                //新数字出现动画
                tiles[x, y].Appera();
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Debug.Write(tiles[i, j].Number + " ");

                    //更新待保存数据
                    UserData.CurrentInstance.Num[i, j] = tiles[i, j].Number;
                }
                Debug.WriteLine("");
            }
            Debug.WriteLine("score = " + score.Text + " best = " + best.Text);
            UserData.CurrentInstance.Score = score.Text;
            UserData.CurrentInstance.Best = best.Text;
        }

        private bool ClearBlank(int o)
        {
            bool hasBlankMove = false;
            if (o == LEFT)
            {
                for (int i = 0; i < 4; i++)
                {
                    int t = 0;
                    for (int j = 0; j < 4; j++)
                    {
                        if (0 != tiles[i, j].Number)
                        {
                            num[i, t++] = tiles[i, j].Number;
                        }
                    }
                }
            }//LEFT

            if (o == RIGHT)
            {
                for (int i = 0; i < 4; i++)
                {
                    int t = 3;
                    for (int j = 3; j >= 0; j--)
                    {
                        if (0 != tiles[i, j].Number)
                        {
                            num[i, t--] = tiles[i, j].Number;
                        }
                    }
                }
            }// RIGHT

            if (o == UP)
            {
                for (int j = 0; j < 4; j++)
                {
                    int t = 0;
                    for (int i = 0; i < 4; i++)
                    {
                        if (0 != tiles[i, j].Number)
                        {
                            num[t++, j] = tiles[i, j].Number;
                        }
                    }
                }
            }// UP

            if (o == DOWN)
            {
                for (int j = 0; j < 4; j++)
                {
                    int t = 3;
                    for (int i = 3; i >= 0; i--)
                    {
                        if (0 != tiles[i, j].Number)
                        {
                            // hasmove = true;
                            num[t--, j] = tiles[i, j].Number;
                        }
                    }
                }
            }//DOWN

            // 更新numbers
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    // 移除空白后与原来不相同,说明有移动
                    if (tiles[i, j].Number != num[i, j])
                        hasBlankMove = true;
                    tiles[i, j].Number = num[i, j];
                }
            }

            return hasBlankMove;
        }

        /// <summary>
        /// 合并数字并更新得分
        /// </summary>
        /// <param name="o">方向</param>
        /// <param name="s">得分</param>
        /// <returns></returns>
        private bool AddNumber(int o, ref int s)
        {

            bool hasAddMove = false;
            if (o == LEFT)
            {
                Debug.WriteLine("←");
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (tiles[i, j].Number == tiles[i, j + 1].Number
                                && tiles[i, j].Number != 0)
                        {
                            //放大的动画
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i, j + 1].Number;
                            s += tiles[i, j].Number;
                            hasAddMove = true;
                            for (int t = j + 1; t < 3; t++)
                            {
                                tiles[i, t].Number = tiles[i, t + 1].Number;
                            }
                            tiles[i, 3].Number = 0;
                        }
                    }
                }// 每行

            }// LEFT

            if (o == RIGHT)
            {
                Debug.WriteLine("→");
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 3; j > 0; j--)
                    {
                        if (tiles[i, j].Number == tiles[i, j - 1].Number
                                && tiles[i, j].Number != 0)
                        {
                            //放大的动画
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i, j - 1].Number;
                            s += tiles[i, j].Number;
                            hasAddMove = true;
                            for (int t = j - 1; t > 0; t--)
                            {
                                tiles[i, t].Number = tiles[i, t - 1].Number;
                            }
                            tiles[i, 0].Number = 0;
                        }
                    }
                }
            }// RIGHT

            if (o == UP)
            {
                Debug.WriteLine("↑");
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (tiles[i, j].Number == tiles[i + 1, j].Number
                                && tiles[i, j].Number != 0)
                        {
                            //放大的动画
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i + 1, j].Number;
                            hasAddMove = true;
                            s += tiles[i, j].Number;
                            for (int t = i + 1; t < 3; t++)
                            {
                                tiles[t, j].Number = tiles[t + 1, j].Number;
                            }
                            tiles[3, j].Number = 0;
                        }
                    }
                }
            }// UP

            if (o == DOWN)
            {
                Debug.WriteLine("↓");
                for (int j = 0; j < 4; j++)
                {
                    for (int i = 3; i > 0; i--)
                    {
                        if (tiles[i, j].Number == tiles[i - 1, j].Number
                                && tiles[i, j].Number != 0)
                        {
                            //放大的动画
                            tiles[i, j].Zoom();

                            tiles[i, j].Number += tiles[i - 1, j].Number;
                            hasAddMove = true;
                            s += tiles[i, j].Number;
                            for (int t = i - 1; t > 0; t--)
                            {
                                tiles[t, j].Number = tiles[t - 1, j].Number;
                            }
                            tiles[0, j].Number = 0;
                        }
                    }
                }
            }//DOWN

            return hasAddMove;
        }

        private bool IsGameOver()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (tiles[i, j].Number == 0)
                    {
                        return false;// 还有空位置
                    }
                    else
                    {
                        if (tiles[i, j].Number == tiles[i, j + 1].Number
                                || tiles[i, j].Number == tiles[i + 1, j].Number)
                        {
                            return false;// 有相等的块
                        }
                    }
                }
                /**
                 * 外层大for循环只判断了3*3格，这里判断最外几个数字(下面x处)
                 * 
                 * <pre>
                 * · · · x
                 * · · · x
                 * · · · x
                 * x x x x
                 * </pre>
                 */
                if (tiles[3, i].Number == tiles[3, i + 1].Number
                        || tiles[i, 3].Number == tiles[i + 1, 3].Number
                        || tiles[3, i].Number == 0 || tiles[i, 3].Number == 0)
                {
                    return false;// 有相等的块或边上有空白块
                }
            }
            return true; // 没有了空位置了,也没有相邻相等的数字
        }

        private async void IfGameOver()
        {
            if (IsGameOver())
            {

                ContentDialog dialog = new ContentDialog()
                {
                    Title = "游戏结束", //标题
                    Content = "\n已无法合并任何数字\n您的得分为:" + score.Text + ",最高分为:" + best.Text + ".",//内容
                    FullSizeDesired = false,  //是否全屏展示
                    PrimaryButtonText = "新游戏",//第一个按钮内容
                    SecondaryButtonText = "返回"
                };
                try
                {
                    var result = await dialog.ShowAsync();
                    if (ContentDialogResult.Primary == result)
                    {
                        NewGame(times++);
                    }
                }
                catch (Exception ex) { Debug.WriteLine(ex.Message); }
            }
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGame(times++);
        }

        private void Init()
        {
            //重置为零
            foreach (Tile t in tiles)
            {
                t.Number = 0;
            }
            Random random = new Random();
            int a = 4;
            if (random.Next(0, 10) != 0) a = 2;
            int x1 = random.Next(0, 4),
                y1 = random.Next(0, 4);
            int x2, y2;
            do
            {
                x2 = random.Next(0, 4);
                y2 = random.Next(0, 4);
            } while (x1 == x2 && y1 == y2);
            //其中一个为2，另一个为2(90%)或4(10%)
            tiles[x1, y1].Number = 2;
            tiles[x2, y2].Number = a;
            score.Text = "0";
        }

        private void NewGame(int nth)
        {
            //刚打开游戏，需要读取上次游戏(如果有)
            if (nth == 0)
            {
                score.Text = UserData.CurrentInstance.Score;
                best.Text = UserData.CurrentInstance.Best;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        tiles[i, j].Number = UserData.CurrentInstance.Num[i, j];
                    }
                }
                if (best.Text.Equals("0")) { Init(); }
            }
            else Init();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //http://www.cnblogs.com/ms-uap/p/4536459.html
            //应用打开时是0，0
            double h, w;
            Debug.WriteLine("SizeChanged:PreviousSize=" + e.PreviousSize + " NewSize=" + e.NewSize);
            if (e.PreviousSize.Width == 0)
            {
                h = main.ActualHeight;
                w = main.ActualWidth;
                border.Width = (h > w) ? w : h;
                border.Height = (h > w) ? w : h;
                return;
            }

            Size size = e.NewSize;
            h = size.Height - 200;
            w = size.Width * 0.8;
            border.Width = (h > w) ? w : h;
            border.Height = (h > w) ? w : h;

        }

        private async void AppBarButton_Like_Click(object sender, RoutedEventArgs e)
        {
            // 在商店中评论指定的 app 
            // bool success = await Launcher.LaunchUriAsync(new Uri("zune:reviewapp?appid=dcdd7004-064a-4c04-ad22-eae725f8ffb1"));
            //http://www.bcmeng.com/jiqiao/
            //https://msdn.microsoft.com/zh-cn/library/windows/apps/windows.applicationmodel.store.currentapp.appid
            try
            {
                //https://msdn.microsoft.com/zh-cn/library/windows/apps/windows.applicationmodel.store.currentapp.aspx
                //AppID要在上架后才有
                await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9NBLGGH68BB0"));
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }
        #region
        //这shi一样的配色,我深深的怀疑自己的审美,还是注释掉好了.
        SolidColorBrush darkgray = new SolidColorBrush(Colors.DarkGray);
        SolidColorBrush white = new SolidColorBrush(Colors.White);
        SolidColorBrush lightgray = new SolidColorBrush(Colors.LightGray);
        SolidColorBrush orange = new SolidColorBrush(Colors.Orange);
        SolidColorBrush darkorange = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.DarkOrange);
        SolidColorBrush txtb = new SolidColorBrush(Color.FromArgb(255, 90, 100, 50));

        private void AppBarToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Checked");
            //夜间
            this.Background = darkgray;
            this.Foreground = lightgray;
            commandBar.Background = new SolidColorBrush(Color.FromArgb(255, 60, 35, 35));
            commandBar.Foreground = lightgray;
            page.Background = new SolidColorBrush(Color.FromArgb(255, 30, 15, 20));

            logo.Background = txtb;
            logof.Foreground = lightgray;
            scoreb.Background = txtb;
            score.Foreground = lightgray;
            score0.Foreground = lightgray;
            bestb.Background = txtb;
            best.Foreground = lightgray;
            best0.Foreground = lightgray;

        }

        private void AppBarToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Unchecked");
            //日间
            this.Background = orange;
            this.Foreground = white;
            commandBar.Background = orange;
            commandBar.Foreground = white;
            page.Background = new SolidColorBrush(Color.FromArgb(255, 250, 248, 239));

            logo.Background = orange;
            scoreb.Background = darkorange;
            bestb.Background = darkorange;
            logof.Foreground = white;
            score.Foreground = white;
            score0.Foreground = white;
            best.Foreground = white;
            best0.Foreground = white;

        }
        #endregion
    }
}
