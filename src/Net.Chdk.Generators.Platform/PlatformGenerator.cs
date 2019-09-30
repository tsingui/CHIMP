﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Net.Chdk.Generators.Platform
{
    sealed class PlatformGenerator : IPlatformGenerator
    {
        private IEnumerable<IInnerPlatformGenerator> Generators { get; }

        public PlatformGenerator(IEnumerable<IInnerPlatformGenerator> generators)
        {
            Generators = generators;
        }

        public string? GetPlatform(uint modelId, string[] models, string? category = null, bool isCanon = false)
        {
            if (models == null)
                throw new ArgumentNullException(nameof(models));

            if (models.Length == 0)
                throw new ArgumentException("At least one model name is required", nameof(models));

            if (models.Any(string.IsNullOrEmpty))
                throw new ArgumentException("Model names cannot be null or empty", nameof(models));

            if (isCanon)
                for (int i = 0; i < models.Length; i++)
                    models[i] = models[i].TrimStart("Canon ");

            return Generators
                .Where(g => category?.Equals(g.CategoryName) != false)
                .Select(g => g.GetPlatform(modelId, models))
                .FirstOrDefault(r => r != null);
        }
    }
}
