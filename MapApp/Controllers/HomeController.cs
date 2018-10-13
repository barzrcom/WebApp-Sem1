
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MapApp.Models.LocationModels;
using MapApp.Data;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using MapApp.Models.CommentsModels;
using Accord.MachineLearning.Bayes;
using Accord.Statistics.Distributions.Univariate;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Analysis;
using Accord.Math.Distances;
using Accord.Statistics.Kernels;
using Accord.MachineLearning;

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

        class CategoryRate
        {
            public LocationCategory lc { get; set; }
            public float l_rating { get; set; }
            public float c_rating { get; set; }
        }
        public async Task<IActionResult> ML(int? id)
        {

            List<double[]> inputs = new List<double[]>();
            inputs.Add(new double[] { 0, 5 });
            inputs.Add(new double[] { 0, 4 });
            inputs.Add(new double[] { 1, 1 });
            inputs.Add(new double[] { 2, 1 });


            //inputs.Append<double[]>(new double[] { 52, 0 }); //not working

            var comments = await _context.Comment.ToListAsync();
            var locations = await _context.Location.ToListAsync();


            var result =
                from c in comments
                join l in locations on c.Location equals l.ID
                where c.User.Equals(User.Identity.Name)
                select new CategoryRate { lc=l.Category, l_rating = l.Rating, c_rating = c.Rating };

            //result.ToList().ForEach(r => inputs.Append<double[]>(new double[] { r.Category.GetHashCode(), r.C_Rating }));

            List<double[]> test = new List<double[]>();
            test.Add(new double[] { 0, 1 });
            test.Add(new double[] { 0, 2 });
            test.Add(new double[] { 0, 3 });
            test.Add(new double[] { 0, 4 });
            test.Add(new double[] { 1, 5 });
            test.Add(new double[] { 1, 2 });



            int[] outputs = { 1, 1, 0, 0 };

            // Create a new Sequential Minimal Optimization (SMO) learning 
            // algorithm and estimate the complexity parameter C from data
            var teacher = new SequentialMinimalOptimization<Gaussian>()
            {
                UseComplexityHeuristic = true,
                UseKernelEstimation = true // estimate the kernel from the data
            };

            // Teach the vector machine
            var svm = teacher.Learn(inputs.ToArray(), outputs);

            // Classify the samples using the model
            bool[] answers = svm.Decide(test.ToArray());

            return null;

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
