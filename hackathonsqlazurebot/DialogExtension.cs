using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace hackathonsqlazurebot
{
    public static class DialogExtension
    {
        public static string GetEntityOriginalText(this EntityRecommendation recommendation, string query)
        {
            if (recommendation.StartIndex.HasValue && recommendation.EndIndex.HasValue)
            {
                return query.Substring(recommendation.StartIndex.Value, recommendation.EndIndex.Value - recommendation.StartIndex.Value + 1);
            }

            return null;
        }
    }
}