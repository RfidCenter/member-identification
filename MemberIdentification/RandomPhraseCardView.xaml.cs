using System.Windows;
using System.Windows.Controls;

namespace MemberIdentification
{
    /// <summary>
    ///     Interaction logic for RandomPhraseCardView.xaml
    /// </summary>
    public partial class RandomPhraseCardView : UserControl
    {
        public static readonly DependencyProperty PersonalCardProperty = DependencyProperty.Register("PersonalCard",
                                                                                                     typeof(PersonalCardRecord),
                                                                                                     typeof(RandomPhraseCardView),
                                                                                                     new PropertyMetadata(default(PersonalCardRecord), PersonalCardPropertyChanged));

        public RandomPhraseCardView()
        {
            this.InitializeComponent();
        }

        public PersonalCardRecord PersonalCard
        {
            get { return (PersonalCardRecord) this.GetValue(PersonalCardProperty); }
            set { this.SetValue(PersonalCardProperty, value); }
        }

        private static void PersonalCardPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as RandomPhraseCardView;
            if (view == null)
                return;

            var record = e.NewValue as PersonalCardRecord;
            if (record == null)
                return;

            view.PhraseBlock.Text = PhraseChooser.Instance.GeneratePhrase(record);
        }
    }
}