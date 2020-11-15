using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Quizzy.Models
{
    public class TriviaCategory
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }


    public partial class ResponseCategories
    {
        [JsonProperty("trivia_categories")]
        public TriviaCategory[] TriviaCategories { get; set; }
    }
}
