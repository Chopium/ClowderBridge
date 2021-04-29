using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClowderHelper
{
    static public JSONObject CreateMetadataPost( JSONObject content, string extractorName="unity", string extractorID="https://clowder.ncsa.illinois.edu/api/extractors/ncsa.unity") {
        JSONObject obj = new JSONObject();
        JSONObject agent = new JSONObject();
        JSONObject context = new JSONObject();
        JSONObject contextInfo = new JSONObject();

        agent.AddField("@type", "cat:extractor");
        agent.AddField("name", extractorName);
        agent.AddField("extractor_id", extractorID);


        context.Add("https://clowder.ncsa.illinois.edu/contexts/metadata.jsonld");
        // contextInfo.AddField("Alphabetic", "https://cyprus.ncsa.illinois.edu/terms/alphabetic.json");
        // context.Add(contextInfo);

        obj.AddField("@context", context);
        obj.AddField("agent", agent );
        obj.AddField("content", content );

        return obj;
    }
}
