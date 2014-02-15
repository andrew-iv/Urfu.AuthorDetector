namespace Urfu.AuthorDetector.Common.MetricProvider
{
    public class WordsAndGrammsMetricProvider : BaseAllPostMetricProvider
    {
        private string[] _useNgramms;
        private string[] _useWords;
        private string[] _grammemes;
        /*private readonly string[] _useNgramms = new[] { " не", " по", "то ", "не ", " пр", " на", "ть ", " и ", " в ", " то", 
            " чт", "ост", "это", " эт", "ли ", " та", "ак ", "ени", ", ч", " с ", " а ", ", н", " - ", "ты ", ", к", "е, ", " я ", 
            "ние", "и, ", ", а", "ия ", ", т", ", п", "ов ", " ты", " ви", ", в", ". н", "-то", ", и", "кот", "ну ", "ела", "ешь", 
            "ще ", "нуж", "шь ", "ал ", 
            "льз", "ть.", ". п", "се ", "о. ", "е. ", ". а", " :)", "те ", "всё", ". и", ". т" };


        private readonly string[] _useWords = new[] { "если", "чтобы", "нибудь", "поэтому", "линукс", "ведь", "этому", "программ", "андроид" };*/

        public WordsAndGrammsMetricProvider(string[] useNgramms, string[] useWords, string[] grammemes)
        {
            _useNgramms = useNgramms;
            _useWords = useWords;
            _grammemes = grammemes;
        }

        protected override string[] UseNgramms
        {
            get { return _useNgramms; }
        }

        protected override string[] UseWords
        {
            get { return _useWords; }
        }

        protected override string[] Grammemes
        {
            get { return _grammemes; }
        }
    }




    public class SomeWordsAndGrammsMetricProvider : BaseAllPostMetricProvider
    {
        private readonly string[] _useNgramms = new [] { " не", " по", "то ", "не ", " пр", " на", "ть ", " и ", " в ", " то", 
            " чт", "ост", "это", " эт", "ли ", " та", "ак ", "ени", ", ч", " с ", " а ", ", н", " - ", "ты ", ", к", "е, ", " я ", 
            "ние", "и, ", ", а", "ия ", ", т", ", п", "ов ", " ты", " ви", ", в", ". н", "-то", ", и", "кот", "ну ", "ела", "ешь", 
            "ще ", "нуж", "шь ", "ал ", 
            "льз", "ть.", ". п", "се ", "о. ", "е. ", ". а", " :)", "те ", "всё", ". и", ". т" };


        private readonly string[] _useWords = new []{"если", "чтобы", "нибудь", "поэтому", "линукс", "ведь", "этому", "программ", "андроид"};

        protected override string[] UseNgramms
        {
            get { return _useNgramms; }
        }

        protected override string[] UseWords
        {
            get { return _useWords; }
        }

        protected override string[] Grammemes
        {
            get { return null; }
        }
    }
}