using Ninject;
using Opcorpora.Dictionary;
using System.Linq;

namespace Urfu.AuthorDetector.Common
{

    public class AllMetricProvider : BaseMetricProvider
    {
        protected override string[] UseNgramms
        {
            get { return StaticVars.Top3Gramms; }
        }

        protected override string[] UseWords
        {
            get { return StaticVars.TopRuWords; }
        }

        protected override string[] Grammemes
        {
            get { return StaticVars.Kernel.Get<IOpcorporaDictionary>().Grammemes.Select(x => x.name).Where(x=>x!=null).ToArray(); }
        }
    }

    public class SelectedMetricProvider : BaseMetricProvider
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
            get { return new string[]{}; }
        }
    }
}