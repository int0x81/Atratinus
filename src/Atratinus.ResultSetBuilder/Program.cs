using Atratinus.Core;
using Atratinus.Core.Models;
using CsvHelper;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;

namespace Atratinus.ResultSetBuilder
{
    class Program
    {
        static readonly MLContext mlContext = new MLContext();
        static PredictionEngine<Supervised, PurposePrediction> predictionEngine;
        static readonly HttpClient http = new HttpClient();

        static void Main()
        {
            // 1. Read in accessions
            IList<InvestmentActivity> accessions = Helper.ReadInAccessions("");

            CreatePredictionEngine(); //Only needed for local ml model prediction

            // 2. Add purpose of intervention predicted by ML
            foreach(var accession in accessions)
            {
                var mlPrediction = GetAzureMLPrediction(accession.PurposeOfTransaction);
                //var mlPrediction = GetLocalModelPrediction(accession.PurposeOfTransaction);

                var purposeOfInterventionType = GetPurposeOfInterventionType(mlPrediction.PurposeOfInterventionTypeId);

                accession.PurposeOfTransactionTypeId = mlPrediction.PurposeOfInterventionTypeId;
                accession.PurposeOfTransactionType = purposeOfInterventionType;
            }

            // 3. Save result as CSV
            Helper.SaveAccessionsAsCSV(accessions, "", false);
        }

        private static string GetPurposeOfInterventionType(int purposeOfInterventionTypeId)
        {
            var purpose = Constants.PurposeOfInterventionTypes.GetValueOrDefault(purposeOfInterventionTypeId);

            if (string.IsNullOrEmpty(purpose))
                throw new ArgumentException("Unkown purpose-of-intervention type");

            return purpose;
        }

        private static void CreatePredictionEngine()
        {
            ITransformer loadedModel = mlContext.Model.Load("", out var modelInputSchema);

            predictionEngine = mlContext.Model.CreatePredictionEngine<Supervised, PurposePrediction>(loadedModel);
        }

        private static PurposePrediction GetAzureMLPrediction(string purposeOfIntervention)
        {
            throw new NotImplementedException();
        }

        private static PurposePrediction GetLocalModelPrediction(string purposeOfIntervention)
        {
            Supervised purpose = new Supervised() { PurposeOfIntervention = purposeOfIntervention };

            var prediction = predictionEngine.Predict(purpose);

            return prediction;
        }
    }
}
