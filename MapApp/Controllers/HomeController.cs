

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MapApp.Models.LocationModels;
using MapApp.Data;
using System.Collections.Generic;

using Accord.MachineLearning.Bayes;
using Accord.Statistics.Distributions.Univariate;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Accord.MachineLearning;
using Accord.Statistics.Distributions.Fitting;

namespace MapApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Recommend()
        {
            var comments = await _context.Comment.ToListAsync();
            var locations = await _context.Location.ToListAsync();

            //Get all locations for training set (locations with a comment of current user)
            var resultTrain =
                from c in comments
                join l in locations on c.Location equals l.ID
                where c.User.Equals(User.Identity.Name)
                select new { l.Category, l_rating = l.Rating, c_rating = c.Rating };

            //Create a list of {Category, Location Rating} for ML input
            List<double[]> trainSetInput = new List<double[]>();
            resultTrain.ToList().ForEach(r => trainSetInput.Add(new double[] { r.Category.GetHashCode(), r.l_rating }));

            //Create a list of Comment rating for ML output - if user's location rate > 3 then positive for this instance
            List<int> trainSetOutput = new List<int>();
            resultTrain.ToList().ForEach(r => trainSetOutput.Add((r.c_rating > 3) ? 1 : 0));

            //Get all locations for test set (locations that published by another user)
            var resultTest =
                from l in locations
                where !l.User.Equals(User.Identity.Name)
                select new { l.Category, l.Rating, l.ID };

            //Create a list of {Category, Location Rating} for test set
            List<double[]> testSet = new List<double[]>();
            resultTest.ToList().ForEach(r => testSet.Add(new double[] { r.Category.GetHashCode(), r.Rating }));

            //Create a list of all potential recommend locations IDs
            List<int> locationsID = new List<int>();
            resultTest.ToList().ForEach(r => locationsID.Add(r.ID));

            bool[] answers = ML_SVM(trainSetInput, trainSetOutput, testSet);

            //Build a recommends location based on SVM result
            List<Location> locationRecommends = new List<Location>();
            for (var i = 0; i < answers.Count(); i++)
            {
                if (answers[i])
                    locationRecommends.Add(locations.Where(s => s.ID == locationsID[i]).SingleOrDefault());
            }

            return View(locations);
        }

        // ML Using Naive Bayes
        // In our problem, we have 2 classes (samples can be either positive or negative), and 2 inputs (x and y coordinates).
        private int[] ML_NaiveBayes(List<double[]> trainSetInput, List<int> trainSetOutput, List<double[]> testSet)
        {
            // Create a Naive Bayes learning algorithm
            var teacher = new NaiveBayesLearning<NormalDistribution>();

            teacher.Options.InnerOption = new NormalOptions()
            {
                Regularization = 1e-12
            };

            // Use the learning algorithm to learn
            var nb = teacher.Learn(trainSetInput.ToArray(), trainSetOutput.ToArray());

            // Classify the samples using the model
            return nb.Decide(testSet.ToArray());
        }

        // ML Using SVM
        private bool[] ML_SVM(List<double[]> trainSetInput, List<int> trainSetOutput, List<double[]> testSet)
        {
            // Create a new Sequential Minimal Optimization (SMO) learning 
            // algorithm and estimate the complexity parameter C from data
            var teacher = new SequentialMinimalOptimization<Gaussian>()
            {
                UseComplexityHeuristic = true,
                UseKernelEstimation = true // estimate the kernel from the data
            };

            // Teach the vector machine
            var svm = teacher.Learn(trainSetInput.ToArray(), trainSetOutput.ToArray()); 

            // Classify the samples using the model
            return svm.Decide(testSet.ToArray());
        }


        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
