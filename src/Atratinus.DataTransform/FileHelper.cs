﻿using Atratinus.Core.Models;
using Atratinus.DataTransform.Models;
using Atratinus.DataTransform.Models.Maps;
using CsvHelper;
using CsvHelper.Configuration;
using Serilog;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace Atratinus.DataTransform
{
    /// <summary>
    /// Provides multiple helper functions for file I/O operations.
    /// </summary>
    static class FileHelper
    {
        /// <summary>
        /// Reads in the investment activities that was provides by Clea as csv file.
        /// </summary>
        /// <param name="filePath">The path to the csv file.</param>
        /// <returns>A dictionary with all red in investments with the accession number as key.</returns>
        internal static IReadOnlyDictionary<string, InvestmentActivity> ReadInCleaInvestmentActivitySet(string filePath)
        {
            var investments = new Dictionary<string, InvestmentActivity>();

            StreamReader reader;

            try
            {
                reader = new StreamReader(filePath);
            }
            catch (FileNotFoundException)
            {
                Log.Error($"Unable to load investment activities from file {filePath}. Please make sure that you specified a correct path.");
                return null;
            }
            catch (IOException)
            {
                Log.Error($"Unable to open file {filePath}. Please close the file before running this programm");
                return null;
            }

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null
            };

            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<CleaInvestmentActivityMap>();

            var records = csv.GetRecords<InvestmentActivity>();
            
            foreach (var record in records)
            {
                if (record.AccessionNumber.Length != 20) //SAFETFY MEASUREMENT: some accessions numbers in origin data set are invalid
                    continue;

                investments.TryAdd(record.AccessionNumber, record);
            }
            return investments;
        }

        /// <summary>
        /// Reads in a list of investors and returns two hash tables. They both
        /// contain the same data but have different keys.
        /// </summary>
        internal static InvestorHashTableSet ReadInInvestorData(string filePath)
        {
            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            };

            using var reader = new StreamReader(filePath);
            using var csv = new CsvReader(reader, config);
            csv.Context.RegisterClassMap<InvestorMap>();

            var records = csv.GetRecords<Investor>();
            var firmNameAsKey = new Dictionary<string, Investor>();
            var fundNameAsKey = new Dictionary<string, Investor>();

            foreach (var investor in records.ToList())
            {
                firmNameAsKey.TryAdd(investor.Name.Replace(" ", "").ToLower(), investor);
                fundNameAsKey.TryAdd(investor.FundName.Replace(" ", "").ToLower(), investor);
            }

            return new InvestorHashTableSet() { FirmNameAsKey = firmNameAsKey, FundNameAsKey = fundNameAsKey };
        }

        internal static IReadOnlyDictionary<string, int> ReadInSupervised(string supervisedFilePath, string edgarFolderPath)
        {
            var dictionary = new Dictionary<string, int>();
            IList<SupervisedMeta> supervised = new List<SupervisedMeta>();

            try
            {
                using StreamReader reader = new StreamReader(supervisedFilePath);
                string json = reader.ReadToEnd();
                supervised = JsonSerializer.Deserialize<List<SupervisedMeta>>(json, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            }
            catch (JsonException)
            {
                Log.Warning("Unable to load supervised data. The file was not in correct JSON format.");
            }

            foreach (var datum in supervised)
            {
                if (File.Exists(Path.Combine(edgarFolderPath, $"{datum.AccessionNumber}.txt")))
                    dictionary.Add(datum.AccessionNumber, datum.PurposeOfTransactionTypeId);
                else
                    Log.Warning($"Unable to find EDGAR file for supervised datum with accession number: {datum.AccessionNumber}");
            }

            return dictionary;
        }
    }
}