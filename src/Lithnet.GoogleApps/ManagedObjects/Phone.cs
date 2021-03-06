﻿using Newtonsoft.Json;

namespace Lithnet.GoogleApps.ManagedObjects
{
    public class Phone : CustomTypeObject, IPrimaryCandidateObject
    {
        [JsonIgnore]
        protected override string[] StandardTypes => new string[] { "home", "work", "other", "home_fax", "work_fax", "mobile", "pager", "other_fax", "compain_main", "assistant", "car", "radio", "isdn", "callback", "telex", "tty_tdd", "work_mobile", "work_pager", "main", "grand_central" };

        [JsonProperty("value"), JsonConverter(typeof(JsonNullStringConverter))]
        public string Value { get; set; }

        [JsonProperty("primary")]
        public bool? Primary { get; set; }

        [JsonIgnore]
        public bool IsPrimary => this.Primary ?? false;

        public override bool IsEmpty()
        {
            return this.Value.IsNullOrNullPlaceholder();
        }

        public override string ToString()
        {
            string format = this.IsPrimary ? "{0}:{1} (primary)" : "{0}:{1}";
            return string.Format(format, this.Type, this.Value);
        }
    }
}
