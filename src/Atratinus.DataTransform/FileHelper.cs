﻿using Atratinus.Core.Models;
using Atratinus.DataTransform.Models;
using Atratinus.DataTransform.Models.Maps;
using CsvHelper;
using CsvHelper.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Atratinus.DataTransform
{
    /// <summary>
    /// Provides multiple helper functions for file I/O operations.
    /// </summary>
    static class FileHelper
    {
        internal static string[] EnumerateEDGARFiles(string path)
        {
            string[] files;
            try
            {
                files = Directory.GetFiles(path, "*.txt");
            }
            catch (Exception ex)
            {
                Log.Error($"Unable access EDGAR files in {path}. Error message: {ex.Message}");
                return Array.Empty<string>();
            }

            if (files.Length == 0)
                Log.Warning($"Couldn't find any txt files under {path}");

            return files;
        }

        internal static AlteryxResult ReadInAlteryxData(string filePath)
        {
            var investments = new Dictionary<string, InvestmentActivity>();
            var invalidEntries = new List<string>();

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
            csv.Context.RegisterClassMap<AlteryxResultMap>();

            var records = csv.GetRecords<InvestmentActivity>();

            foreach (var record in records)
            {
                if (!DataValidators.IsValidAccessionNumber(record.AccessionNumber))
                    invalidEntries.Add(record.AccessionNumber);
                else
                    investments.TryAdd(record.AccessionNumber, record);
            }
            return new AlteryxResult { InvestmentActivities = investments, DataWithInvalidAccessionNumber = invalidEntries };
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
            catch(Exception)
            {
                Log.Warning("Unable to load supervised data.");
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

        internal static void SaveTrainingData(IList<Supervised> trainingData, string folderPath)
        {
            string jsonLines = "";

            foreach(var datum in trainingData)
            {
                jsonLines += JsonSerializer.Serialize(datum).Replace(Environment.NewLine, "");
                jsonLines += Environment.NewLine;
            }

            string path = Path.Combine(folderPath, "trainingData.jsonl");

            try
            {
                File.WriteAllText(path, jsonLines);
            }
            catch (Exception ex)
            {
                Log.Warning($"Unable to save training data in {folderPath}. Error message was: {ex.Message}");
            }
        } 

        internal static bool TryCreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    Log.Error($"Unable to create output directory. Error Message: {ex.Message}");
                    return false;
                }
            }

            return true;
        }
    }
}
