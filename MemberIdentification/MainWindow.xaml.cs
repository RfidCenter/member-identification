using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

using DevExpress.Xpf.Core;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;

using MemberIdentification.DatabaseCode;

namespace MemberIdentification
{
    public partial class MainWindow
    {
        private const string NEW_ITEM_NAME = "newItem";
        private const string OLD_ITEM_NAME = "oldItem";

        private readonly NameScope _historyNameScope;

        private int _last = 0;
        private string _lastEpc = string.Empty;

        public MainWindow()
        {
            this.InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.Background = new ImageBrush(new BitmapImage(new Uri(@"resources/background.png", UriKind.Relative)));

                this._historyNameScope = new NameScope();
                NameScope.SetNameScope(this, this._historyNameScope);

                try
                {
                    ConnectionManager.Instance.Initialize();

                    ConnectionManager.Instance.TagsFound += this.ConnectionManagerOnTagsFound;

                    ConnectionHelper.Connect(AutoCreateOption.DatabaseAndSchema);

                    this.Loaded += this.OnLoaded;
                }
                catch (Exception ex)
                {
                    DXMessageBox.Show(ex.Message);
                    Application.Current.Shutdown();
                }
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            this.HistoryStackPanel.MaxHeight = this.ActualHeight - 250;
        }

        private void ConnectionManagerOnTagsFound(object sender, TagsEventArgs args)
        {
            if (this.Dispatcher.CheckAccess())
            {
                this.InvokedTagsFound(args.Tags);
            }
            else
            {
                this.Dispatcher.Invoke(() => this.InvokedTagsFound(args.Tags));
            }
        }

        private void InvokedTagsFound(string[] tags)
        {
            using (var session = new Session())
            {
                using (var usersQuery = new XPCollection<Persona>(session))
                {
                    var found = from user in usersQuery
                                join tag in tags on user.Tid equals tag
                                where tag != this._lastEpc
                                select user;

                    foreach (var user in found)
                    {
                        var card = new PersonalCardRecord(user);

                        this.AddCardView(new GreatingsCardView()
                                         {
                                             PersonalCard = card
                                         });

                        //if (user.SawTimes == 0)
                        //    this.AddCardView(new GreatingsCardView() { PersonalCard = card });
                        //else
                        //    this.AddCardView(new RandomPhraseCardView() { PersonalCard = card });

                        user.SawTimes++;
                        user.Save();

                        this._lastEpc = user.Tid;
                    }
                }
            }
        }

        private void AddCardView(FrameworkElement view)
        {
            var oldCardView = this._historyNameScope.FindName(NEW_ITEM_NAME) as FrameworkElement;

            this._historyNameScope.Clear();

            view.Name = NEW_ITEM_NAME;
            view.Height = 0;
            this.RegisterName(NEW_ITEM_NAME, view);
            this.HistoryStackPanel.Children.Insert(0, view);

            var duration = new Duration(TimeSpan.FromSeconds(1));

            var newItemSizeIn = new DoubleAnimation(0, Properties.Settings.Default.NewCardHeight, duration);
            Storyboard.SetTargetName(newItemSizeIn, view.Name);
            Storyboard.SetTargetProperty(newItemSizeIn, new PropertyPath(HeightProperty));

            var newItemFadeIn = new DoubleAnimation(0, 1, duration);
            Storyboard.SetTargetName(newItemFadeIn, view.Name);
            Storyboard.SetTargetProperty(newItemFadeIn, new PropertyPath(OpacityProperty));

            var storyboard = new Storyboard();
            storyboard.Children.Add(newItemSizeIn);
            storyboard.Children.Add(newItemFadeIn);

            if (oldCardView != null)
            {
                oldCardView.Name = OLD_ITEM_NAME;
                this.RegisterName(oldCardView.Name, oldCardView);

                if (oldCardView is GreatingsCardView)
                {
                    var cardView = (GreatingsCardView) oldCardView;

                    var oldItemSizeOut = new DoubleAnimation(Properties.Settings.Default.NewCardHeight,
                                                             Properties.Settings.Default.OldCardheight,
                                                             duration);
                    Storyboard.SetTargetName(oldItemSizeOut, cardView.Name);
                    Storyboard.SetTargetProperty(oldItemSizeOut, new PropertyPath(HeightProperty));

                    var setGreatingVisibility = new DoubleAnimation(1, 0, duration);
                    Storyboard.SetTargetName(setGreatingVisibility, cardView.Name);
                    Storyboard.SetTargetProperty(setGreatingVisibility, new PropertyPath(GreatingsCardView.GreatingTextOpacityProperty));

                    var stretchAnimation = new ObjectAnimationUsingKeyFrames();
                    stretchAnimation.KeyFrames.Add(new DiscreteObjectKeyFrame(Stretch.None, KeyTime.FromPercent(1)));
                    Storyboard.SetTargetName(stretchAnimation, cardView.Name);
                    Storyboard.SetTargetProperty(stretchAnimation, new PropertyPath(GreatingsCardView.ViewboxesStretchProperty));

                    storyboard.Children.Add(oldItemSizeOut);
                    storyboard.Children.Add(setGreatingVisibility);
                    storyboard.Children.Add(stretchAnimation);
                }
                else if (oldCardView is RandomPhraseCardView)
                {
                    var cardView = (RandomPhraseCardView) oldCardView;

                    var oldItemSizeOut = new DoubleAnimation(Properties.Settings.Default.NewCardHeight, 0, duration);
                    Storyboard.SetTargetName(oldItemSizeOut, cardView.Name);
                    Storyboard.SetTargetProperty(oldItemSizeOut, new PropertyPath(HeightProperty));

                    var oldItemFadeOut = new DoubleAnimation(1, 0, duration);
                    Storyboard.SetTargetName(oldItemFadeOut, cardView.Name);
                    Storyboard.SetTargetProperty(oldItemFadeOut, new PropertyPath(OpacityProperty));

                    storyboard.Children.Add(oldItemSizeOut);
                    storyboard.Children.Add(oldItemFadeOut);
                }
            }

            storyboard.Completed += this.OnAnimationComplete;

            Timeline.SetDesiredFrameRate(storyboard, 25);
            storyboard.Begin(this, HandoffBehavior.SnapshotAndReplace);
        }

        private void OnAnimationComplete(object sender, EventArgs e)
        {
            var random = (from child in this.HistoryStackPanel.Children.Cast<UIElement>()
                          let view = child as RandomPhraseCardView
                          where (view != null) && (view.Height < 1)
                          select child).ToArray();

            foreach (var element in random)
            {
                this.HistoryStackPanel.Children.Remove(element);
            }

            while (this.HistoryStackPanel.Children.Count > Properties.Settings.Default.MaxHistoryItemsCount)
            {
                this.HistoryStackPanel.Children.RemoveAt(this.HistoryStackPanel.Children.Count - 1);
            }
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                using (var session = new Session())
                {
                    using (var personas = new XPCollection<Persona>(session))
                    {
                        var persona = personas[this._last];
                        this.AddCardView(new GreatingsCardView()
                                         {
                                             PersonalCard = new PersonalCardRecord(persona)
                                         });

                        this._last++;

                        if (this._last == personas.Count)
                        {
                            this._last = 0;
                        }
                    }
                }

                e.Handled = true;
            }
        }
    }
}
