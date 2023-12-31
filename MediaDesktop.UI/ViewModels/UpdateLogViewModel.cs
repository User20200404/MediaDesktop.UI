﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MediaDesktop.UI.ViewModels
{
    /// <summary>
    /// Stores update log info.
    /// </summary>
    public class UpdateLogViewModel
    {
        public string Version { get; init; }
        public List<string> Added { get; init; } = new List<string>();
        public List<string> Removed { get; init; } = new List<string>();
        public List<string> Modified { get; init; } = new List<string>();
        public string Notes { get; init; }
        [JsonIgnore]
        public string SummaryText => GetSummaryText();

        private string GetSummaryText()
        {
            StringBuilder builder = new StringBuilder();
            if(Added.Count > 0)
            {
                Added.ForEach(s => builder.AppendLine($"+{s}")); 
                builder.AppendLine();
            }

            if (Removed.Count > 0)
            {
                Removed.ForEach(s => builder.AppendLine($"-{s}"));
                builder.AppendLine();
            }

            if (Modified.Count > 0)
            {
                Modified.ForEach(s => builder.AppendLine($"⚪{s}"));
                builder.AppendLine();
            }
            builder.AppendLine(Notes);
            return builder.ToString();
        }
    }
}
