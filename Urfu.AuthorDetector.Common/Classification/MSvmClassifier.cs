using System.Collections.Generic;
using System.Linq;
using Accord.Statistics.Kernels;
using Urfu.AuthorDetector.Common.MetricProvider;
using Urfu.AuthorDetector.DataLayer;

namespace Urfu.AuthorDetector.Common.Classification
{
    /// <summary>
    /// Svm классификатор
    /// </summary>
    public class MSvmClassifier : IClassifier
    {


        private readonly IDictionary<Author, IEnumerable<string>> _authors;
        private readonly ICommonMetricProvider _commonProvider;
        private readonly bool _probalistic;
        private readonly IKernel _kernel;
        private string[] _allPosts;
        private Author[] _keys;
        private KeyValuePair<int, double[]>[] _authorsMetrics;
        private KeyValuePair<int, double[]> _stdDevs;

        private MultyKSvmStatistics _stat;

        public MSvmClassifier(MSvmClassifierParams mSvmClassifierParams, IDictionary<Author, IEnumerable<string>> authors)
        {
            _authors = authors;
            //_keys = authors.Keys.ToArray();
            _stat = new MultyKSvmStatistics(mSvmClassifierParams, authors);
            if (mSvmClassifierParams.Kernel != null)
                _stat.Kernel = mSvmClassifierParams.Kernel;
                
            if (mSvmClassifierParams.Probalistic)
            {
                _stat.StudyProbalistic = true;
            }
            //_stat.Kernel = _stat.EstimateGaussian;
            _stat.Study();
            _commonProvider = mSvmClassifierParams.CommonProvider;
            _probalistic = mSvmClassifierParams.Probalistic;
            _kernel = mSvmClassifierParams.Kernel;
        }

        
        //private MultyKSvmStatistics

        public void LogResult(bool isSuccess)
        {
        }

        public IEnumerable<Author> Authors { get { return _keys; } }
        
        public Author ClassificatePosts(IEnumerable<string> posts, out bool reliable)
        {
            reliable = true;
            return ClassificatePosts(posts, 1)[0];
        }

        public Author[] ClassificatePosts(IEnumerable<string> posts, int topN)
        {
            return (_probalistic
                        ? _stat.GetProbabilities(posts).OrderByDescending(x => x.Value).Select(x => x.Key)
                        : _stat.GetTops(posts).OrderByDescending(x => x.Value).Select(x => x.Key)).Take(topN).ToArray();
        }

        public string Description { get { return "SvmClassifier"; } }
        public string Name { get { return "SvmClassifier"; } }
        public double ErrorLevel { get; set; }
    }
}