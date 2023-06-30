using Newtonsoft.Json;

namespace A3_API_Project.Models
{
    public class Token
    {
        //[JsonProperty("access_token")]
        public string AccessToken { get; set; }

        //[JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        //[JsonProperty("token_type")]
        public string TokenType { get; set; }
    }
}
