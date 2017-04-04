using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MemberIdentification
{
    /// <summary>
    ///     Interaction logic for PersonalCardView.xaml
    /// </summary>
    public partial class GreatingsCardView : UserControl
    {
        public static readonly DependencyProperty PersonalCardProperty = DependencyProperty.Register("PersonalCard",
                                                                                                     typeof(PersonalCardRecord),
                                                                                                     typeof(GreatingsCardView),
                                                                                                     new PropertyMetadata(default(PersonalCardRecord)));

        public static readonly DependencyProperty GreatingTextOpacityProperty = DependencyProperty.Register("GreatingTextOpacity",
                                                                                                            typeof(double),
                                                                                                            typeof(GreatingsCardView),
                                                                                                            new PropertyMetadata(1.0));

        public static readonly DependencyProperty ViewboxesStretchProperty = DependencyProperty.Register("ViewboxesStretch",
                                                                                                         typeof(Stretch),
                                                                                                         typeof(GreatingsCardView),
                                                                                                         new PropertyMetadata(Stretch.Uniform, ViewboxesStretchPropertyChanged));

        private static void ViewboxesStretchPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var card = d as GreatingsCardView;
            if(card == null)
                return;

            if ((Stretch) e.NewValue == Stretch.None)
            {
                card.InitialsBlock.FontSize = Properties.Settings.Default.InitialsFontSize;
                card.OccupationBlock.FontSize = Properties.Settings.Default.OccupationFontSize;
            }
        }

        public GreatingsCardView()
        {
            this.InitializeComponent();
        }

        public Stretch ViewboxesStretch
        {
            get { return (Stretch) this.GetValue(ViewboxesStretchProperty); }
            set { this.SetValue(ViewboxesStretchProperty, value); }
        }

        public double GreatingTextOpacity
        {
            get { return (double) this.GetValue(GreatingTextOpacityProperty); }
            set { this.SetValue(GreatingTextOpacityProperty, value); }
        }

        public PersonalCardRecord PersonalCard
        {
            get { return (PersonalCardRecord) this.GetValue(PersonalCardProperty); }
            set { this.SetValue(PersonalCardProperty, value); }
        }
    }
}