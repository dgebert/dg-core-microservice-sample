
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace dg.document.db.Repository
{
   
    public abstract class Entity
    {
        public Entity(string type)
        {
            Type = type;
        }
        /// <summary>
        /// Object unique identifier
        /// </summary>
        [Key]
        [JsonProperty("id")]
        public string Id { get; set; }
        /// <summary>
        /// Object type
        /// </summary>
        public string Type { get; private set; }
    }
}
