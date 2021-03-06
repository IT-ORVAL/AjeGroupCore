﻿/**
* Copyright 2017 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using Newtonsoft.Json;

namespace IBM.VCA.Watson.Watson.Model
{
    /// <summary>
    /// Context object
    /// </summary>
    public class Context
    {
        /// <summary>
        /// The unique identifier of the conversation.
        /// </summary>
        [JsonProperty("conversation_id")]
        public string ConversationId { get; set; }

        /// <summary>
        /// Information about the dialog.
        /// </summary>
        [JsonProperty("system")]
        public SystemResponse System { get; set; }




        [JsonProperty("temperature")]
        public string Temperature { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
        
        
        //AJE Group Google Email Reset Password

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("valid")]
        public bool Valid { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

    }
}
