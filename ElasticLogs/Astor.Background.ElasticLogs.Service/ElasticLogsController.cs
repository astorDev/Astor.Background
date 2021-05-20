using System;
using System.Threading.Tasks;
using Astor.Background.Core;
using Astor.Background.Core.Abstractions;
using Astor.Background.Management.Protocol;
using Nest;

namespace Astor.Background.ElasticLogs.Service
{
    public class ElasticLogsController
    {
        public IElasticClient ElasticClient { get; }
        public KibanaClient KibanaClient { get; }

        public ElasticLogsController(IElasticClient elasticClient, KibanaClient kibanaClient)
        {
            this.ElasticClient = elasticClient;
            this.KibanaClient = kibanaClient;
        }
        
        [RabbitMq.Abstractions.SubscribedOn(ExchangeNames.Logs)]
        public async Task SaveAsync(ActionResultCandidate candidate)
        {
            
            var actionFamilyName = ActionId.Parse(candidate.ActionId).Receiver; // needs to be changed for ActionId.Parse when it will be created
            var indexName = $"backgroundlogs-{actionFamilyName.ToLower()}";

            var record = new
            {
                successful = candidate.IsSuccessful, 
                @event = candidate.Event,
                startTime = candidate.StartTime,
                endTime = candidate.EndTime,
                actionId = candidate.ActionId,
                actionFamilyName,
                elapsedMilliseconds = (candidate.EndTime - candidate.StartTime).TotalMilliseconds,
                result = candidate.Result,
                exception = candidate.Exception,
                attemptIndex = candidate.AttemptIndex,
                sourceExchange = candidate.SourceExchange
            };

            await this.ElasticClient.IndexAsync(record, i => i.Index(indexName));
        }

        [SubscribedOnInternal(InternalEventNames.Started)]
        public async Task CreateDashboardAsync()
        {
            var requestJson = @"{
    ""version"": ""7.12.1"",
    ""objects"": [
        {
            ""id"": ""background-logs-dashboard"",
            ""type"": ""dashboard"",
            ""namespaces"": [
                ""default""
            ],
            ""updated_at"": ""2021-05-20T16:06:42.665Z"",
            ""version"": ""WzgxMDMsMl0="",
            ""attributes"": {
                ""title"": ""Background Logs"",
                ""hits"": 0,
                ""description"": """",
                ""panelsJSON"": ""[{\""version\"":\""7.12.1\"",\""type\"":\""visualization\"",\""gridData\"":{\""x\"":0,\""y\"":0,\""w\"":22,\""h\"":12,\""i\"":\""acb4d4e3-9294-44f0-9ff7-4ce4ce5486b8\""},\""panelIndex\"":\""acb4d4e3-9294-44f0-9ff7-4ce4ce5486b8\"",\""embeddableConfig\"":{\""savedVis\"":{\""title\"":\""\"",\""description\"":\""\"",\""type\"":\""heatmap\"",\""params\"":{\""type\"":\""heatmap\"",\""addTooltip\"":true,\""addLegend\"":true,\""enableHover\"":false,\""legendPosition\"":\""bottom\"",\""times\"":[],\""colorsNumber\"":10,\""colorSchema\"":\""Green to Red\"",\""setColorRange\"":false,\""colorsRange\"":[],\""invertColors\"":false,\""percentageMode\"":false,\""valueAxes\"":[{\""show\"":false,\""id\"":\""ValueAxis-1\"",\""type\"":\""value\"",\""scale\"":{\""type\"":\""linear\"",\""defaultYExtents\"":false},\""labels\"":{\""show\"":true,\""rotate\"":0,\""overwriteColor\"":false,\""color\"":\""black\""}}]},\""uiState\"":{\""vis\"":{\""defaultColors\"":{\""0 - 0.2\"":\""rgb(0,104,55)\"",\""0.2 - 0.4\"":\""rgb(26,151,80)\"",\""0.4 - 0.6\"":\""rgb(102,189,99)\"",\""0.6 - 0.8\"":\""rgb(166,217,106)\"",\""0.8 - 1\"":\""rgb(217,239,139)\"",\""1 - 1.2\"":\""rgb(255,255,190)\"",\""1.2 - 1.4\"":\""rgb(254,224,139)\"",\""1.4 - 1.6\"":\""rgb(253,174,97)\"",\""1.6 - 1.8\"":\""rgb(244,109,67)\"",\""1.8 - 2\"":\""rgb(214,47,39)\""}}},\""data\"":{\""aggs\"":[{\""id\"":\""background-logs-index-pattern\"",\""enabled\"":true,\""type\"":\""count\"",\""params\"":{},\""schema\"":\""metric\""},{\""id\"":\""2\"",\""enabled\"":true,\""type\"":\""terms\"",\""params\"":{\""field\"":\""actionFamilyName.keyword\"",\""orderBy\"":\""background-logs-index-pattern\"",\""order\"":\""desc\"",\""size\"":100,\""otherBucket\"":false,\""otherBucketLabel\"":\""Other\"",\""missingBucket\"":false,\""missingBucketLabel\"":\""Missing\""},\""schema\"":\""segment\""},{\""id\"":\""3\"",\""enabled\"":true,\""type\"":\""filters\"",\""params\"":{\""filters\"":[{\""input\"":{\""query\"":\""successful : false \"",\""language\"":\""kuery\""},\""label\"":\""\""}]},\""schema\"":\""group\""}],\""searchSource\"":{\""index\"":\""background-logs-index-pattern\"",\""query\"":{\""query\"":\""\"",\""language\"":\""kuery\""},\""filter\"":[]}}},\""vis\"":{\""defaultColors\"":{\""0\"":\""rgb(0,104,55)\""},\""legendOpen\"":false},\""hidePanelTitles\"":false,\""table\"":null,\""enhancements\"":{}},\""title\"":\""Fails\""},{\""version\"":\""7.12.1\"",\""type\"":\""visualization\"",\""gridData\"":{\""x\"":22,\""y\"":0,\""w\"":26,\""h\"":15,\""i\"":\""c63c7766-ed55-4cb3-b5e5-37010fc63316\""},\""panelIndex\"":\""c63c7766-ed55-4cb3-b5e5-37010fc63316\"",\""embeddableConfig\"":{\""savedVis\"":{\""title\"":\""\"",\""description\"":\""\"",\""type\"":\""metrics\"",\""params\"":{\""id\"":\""61ca57f0-469d-11e7-af02-69e470af7417\"",\""type\"":\""timeseries\"",\""series\"":[{\""id\"":\""61ca57f1-469d-11e7-af02-69e470af7417\"",\""color\"":\""rgba(71,245,59,1)\"",\""split_mode\"":\""everything\"",\""split_color_mode\"":\""kibana\"",\""metrics\"":[{\""id\"":\""61ca57f2-469d-11e7-af02-69e470af7417\"",\""type\"":\""count\""}],\""separate_axis\"":0,\""axis_position\"":\""right\"",\""formatter\"":\""number\"",\""chart_type\"":\""line\"",\""line_width\"":1,\""point_size\"":1,\""fill\"":0.5,\""stacked\"":\""none\"",\""label\"":\""Total\"",\""filter\"":{\""query\"":\""successful : true or successful : false \"",\""language\"":\""kuery\""}},{\""id\"":\""9c21c670-b955-11eb-9fef-39ded4560849\"",\""color\"":\""rgba(251,27,79,1)\"",\""split_mode\"":\""filter\"",\""metrics\"":[{\""id\"":\""9c21c671-b955-11eb-9fef-39ded4560849\"",\""type\"":\""count\""}],\""separate_axis\"":0,\""axis_position\"":\""right\"",\""formatter\"":\""number\"",\""chart_type\"":\""line\"",\""line_width\"":1,\""point_size\"":1,\""fill\"":0.5,\""stacked\"":\""none\"",\""label\"":\""Fails\"",\""filter\"":{\""query\"":\""successful : false \"",\""language\"":\""kuery\""}}],\""time_field\"":\""\"",\""index_pattern\"":\""backgroundlogs-*\"",\""interval\"":\""\"",\""axis_position\"":\""left\"",\""axis_formatter\"":\""number\"",\""axis_scale\"":\""normal\"",\""show_legend\"":1,\""show_grid\"":1,\""tooltip_mode\"":\""show_all\"",\""default_index_pattern\"":\""kibana_sample_data_logs\"",\""default_timefield\"":\""timestamp\"",\""isModelInvalid\"":false},\""uiState\"":{},\""data\"":{\""aggs\"":[],\""searchSource\"":{\""query\"":{\""query\"":\""\"",\""language\"":\""kuery\""},\""filter\"":[]}}},\""hidePanelTitles\"":false,\""enhancements\"":{}},\""title\"":\""Load\""},{\""version\"":\""7.12.1\"",\""type\"":\""visualization\"",\""gridData\"":{\""x\"":0,\""y\"":12,\""w\"":22,\""h\"":12,\""i\"":\""8c18b77e-45ad-4586-8182-63bc841b95c0\""},\""panelIndex\"":\""8c18b77e-45ad-4586-8182-63bc841b95c0\"",\""embeddableConfig\"":{\""savedVis\"":{\""title\"":\""\"",\""description\"":\""\"",\""type\"":\""heatmap\"",\""params\"":{\""type\"":\""heatmap\"",\""addTooltip\"":true,\""addLegend\"":true,\""enableHover\"":false,\""legendPosition\"":\""bottom\"",\""times\"":[],\""colorsNumber\"":10,\""colorSchema\"":\""Greens\"",\""setColorRange\"":false,\""colorsRange\"":[],\""invertColors\"":false,\""percentageMode\"":false,\""valueAxes\"":[{\""show\"":false,\""id\"":\""ValueAxis-1\"",\""type\"":\""value\"",\""scale\"":{\""type\"":\""linear\"",\""defaultYExtents\"":false},\""labels\"":{\""show\"":true,\""rotate\"":0,\""overwriteColor\"":false,\""color\"":\""black\""}}]},\""uiState\"":{\""vis\"":{\""defaultColors\"":{\""0 - 0.3\"":\""rgb(247,252,245)\"",\""0.3 - 0.6\"":\""rgb(233,247,228)\"",\""0.6 - 0.9\"":\""rgb(211,238,205)\"",\""0.9 - 1.2\"":\""rgb(184,227,177)\"",\""1.2 - 1.5\"":\""rgb(152,213,148)\"",\""1.5 - 1.8\"":\""rgb(116,196,118)\"",\""1.8 - 2.1\"":\""rgb(75,176,98)\"",\""2.1 - 2.4\"":\""rgb(47,152,79)\"",\""2.4 - 2.7\"":\""rgb(21,127,59)\"",\""2.7 - 3\"":\""rgb(0,100,40)\""}}},\""data\"":{\""aggs\"":[{\""id\"":\""background-logs-index-pattern\"",\""enabled\"":true,\""type\"":\""count\"",\""params\"":{},\""schema\"":\""metric\""},{\""id\"":\""2\"",\""enabled\"":true,\""type\"":\""terms\"",\""params\"":{\""field\"":\""actionFamilyName.keyword\"",\""orderBy\"":\""background-logs-index-pattern\"",\""order\"":\""desc\"",\""size\"":100,\""otherBucket\"":false,\""otherBucketLabel\"":\""Other\"",\""missingBucket\"":false,\""missingBucketLabel\"":\""Missing\""},\""schema\"":\""segment\""},{\""id\"":\""3\"",\""enabled\"":true,\""type\"":\""filters\"",\""params\"":{\""filters\"":[{\""input\"":{\""query\"":\""successful : true \"",\""language\"":\""kuery\""},\""label\"":\""\""}]},\""schema\"":\""group\""}],\""searchSource\"":{\""index\"":\""background-logs-index-pattern\"",\""query\"":{\""query\"":\""\"",\""language\"":\""kuery\""},\""filter\"":[]}}},\""vis\"":{\""defaultColors\"":{\""0 - 45\"":\""rgb(0,104,55)\"",\""45 - 90\"":\""rgb(26,151,80)\"",\""90 - 135\"":\""rgb(102,189,99)\"",\""135 - 180\"":\""rgb(166,217,106)\"",\""180 - 225\"":\""rgb(217,239,139)\"",\""225 - 270\"":\""rgb(255,255,190)\"",\""270 - 315\"":\""rgb(254,224,139)\"",\""315 - 360\"":\""rgb(253,174,97)\"",\""360 - 405\"":\""rgb(244,109,67)\"",\""405 - 450\"":\""rgb(214,47,39)\""},\""legendOpen\"":false},\""hidePanelTitles\"":false,\""table\"":null,\""enhancements\"":{}},\""title\"":\""Successes\""},{\""version\"":\""7.12.1\"",\""type\"":\""lens\"",\""gridData\"":{\""x\"":22,\""y\"":15,\""w\"":9,\""h\"":9,\""i\"":\""d1da0dfc-19fa-4a8c-ac66-533ae08e334a\""},\""panelIndex\"":\""d1da0dfc-19fa-4a8c-ac66-533ae08e334a\"",\""embeddableConfig\"":{\""attributes\"":{\""title\"":\""\"",\""type\"":\""lens\"",\""visualizationType\"":\""lnsMetric\"",\""state\"":{\""datasourceStates\"":{\""indexpattern\"":{\""layers\"":{\""9ef4ec8d-c4c1-4b30-8e58-4f44f1173779\"":{\""columns\"":{\""a79b4e23-20f9-497e-be7b-2a210c50ad99\"":{\""label\"":\""Average of elapsedMilliseconds\"",\""dataType\"":\""number\"",\""operationType\"":\""avg\"",\""sourceField\"":\""elapsedMilliseconds\"",\""isBucketed\"":false,\""scale\"":\""ratio\"",\""params\"":{\""format\"":{\""id\"":\""number\"",\""params\"":{\""decimals\"":0}}}}},\""columnOrder\"":[\""a79b4e23-20f9-497e-be7b-2a210c50ad99\""],\""incompleteColumns\"":{}}}}},\""visualization\"":{\""layerId\"":\""9ef4ec8d-c4c1-4b30-8e58-4f44f1173779\"",\""accessor\"":\""a79b4e23-20f9-497e-be7b-2a210c50ad99\""},\""query\"":{\""query\"":\""\"",\""language\"":\""kuery\""},\""filters\"":[]},\""references\"":[{\""type\"":\""index-pattern\"",\""id\"":\""background-logs-index-pattern\"",\""name\"":\""indexpattern-datasource-current-indexpattern\""},{\""type\"":\""index-pattern\"",\""id\"":\""background-logs-index-pattern\"",\""name\"":\""indexpattern-datasource-layer-9ef4ec8d-c4c1-4b30-8e58-4f44f1173779\""}]},\""hidePanelTitles\"":false,\""enhancements\"":{}},\""title\"":\""Performance\""},{\""version\"":\""7.12.1\"",\""type\"":\""lens\"",\""gridData\"":{\""x\"":31,\""y\"":15,\""w\"":8,\""h\"":9,\""i\"":\""e4d3f04e-02c2-46d7-8fcf-473fb2fc0682\""},\""panelIndex\"":\""e4d3f04e-02c2-46d7-8fcf-473fb2fc0682\"",\""embeddableConfig\"":{\""attributes\"":{\""title\"":\""\"",\""type\"":\""lens\"",\""visualizationType\"":\""lnsPie\"",\""state\"":{\""datasourceStates\"":{\""indexpattern\"":{\""layers\"":{\""7792e0ba-09e8-4c13-b23b-2a9b04d5f7e7\"":{\""columns\"":{\""3daa5e64-2cfc-4254-b959-b0bc6f0cc08d\"":{\""label\"":\""Top values of successful\"",\""dataType\"":\""boolean\"",\""operationType\"":\""terms\"",\""scale\"":\""ordinal\"",\""sourceField\"":\""successful\"",\""isBucketed\"":true,\""params\"":{\""size\"":5,\""orderBy\"":{\""type\"":\""column\"",\""columnId\"":\""5bcc9d83-a5b0-4f47-8e34-098697b626aa\""},\""orderDirection\"":\""desc\"",\""otherBucket\"":true,\""missingBucket\"":false}},\""5bcc9d83-a5b0-4f47-8e34-098697b626aa\"":{\""label\"":\""Count of records\"",\""dataType\"":\""number\"",\""operationType\"":\""count\"",\""isBucketed\"":false,\""scale\"":\""ratio\"",\""sourceField\"":\""Records\""}},\""columnOrder\"":[\""3daa5e64-2cfc-4254-b959-b0bc6f0cc08d\"",\""5bcc9d83-a5b0-4f47-8e34-098697b626aa\""],\""incompleteColumns\"":{}}}}},\""visualization\"":{\""shape\"":\""donut\"",\""palette\"":{\""type\"":\""palette\"",\""name\"":\""status\""},\""layers\"":[{\""layerId\"":\""7792e0ba-09e8-4c13-b23b-2a9b04d5f7e7\"",\""groups\"":[\""3daa5e64-2cfc-4254-b959-b0bc6f0cc08d\""],\""metric\"":\""5bcc9d83-a5b0-4f47-8e34-098697b626aa\"",\""numberDisplay\"":\""percent\"",\""categoryDisplay\"":\""default\"",\""legendDisplay\"":\""default\"",\""nestedLegend\"":false}]},\""query\"":{\""query\"":\""\"",\""language\"":\""kuery\""},\""filters\"":[]},\""references\"":[{\""type\"":\""index-pattern\"",\""id\"":\""background-logs-index-pattern\"",\""name\"":\""indexpattern-datasource-current-indexpattern\""},{\""type\"":\""index-pattern\"",\""id\"":\""background-logs-index-pattern\"",\""name\"":\""indexpattern-datasource-layer-7792e0ba-09e8-4c13-b23b-2a9b04d5f7e7\""}]},\""enhancements\"":{},\""hidePanelTitles\"":false},\""title\"":\""Successes Rate\""},{\""version\"":\""7.12.1\"",\""type\"":\""lens\"",\""gridData\"":{\""x\"":39,\""y\"":15,\""w\"":9,\""h\"":9,\""i\"":\""11aa4beb-de40-4fa0-8b96-9659e9144085\""},\""panelIndex\"":\""11aa4beb-de40-4fa0-8b96-9659e9144085\"",\""embeddableConfig\"":{\""attributes\"":{\""title\"":\""\"",\""type\"":\""lens\"",\""visualizationType\"":\""lnsMetric\"",\""state\"":{\""datasourceStates\"":{\""indexpattern\"":{\""layers\"":{\""598e6823-9694-4528-8f4a-c68514e4a613\"":{\""columns\"":{\""c45a9309-1774-49e2-b3a9-a77f4f8d9e2d\"":{\""label\"":\""Count of records\"",\""dataType\"":\""number\"",\""operationType\"":\""count\"",\""isBucketed\"":false,\""scale\"":\""ratio\"",\""sourceField\"":\""Records\""}},\""columnOrder\"":[\""c45a9309-1774-49e2-b3a9-a77f4f8d9e2d\""],\""incompleteColumns\"":{}}}}},\""visualization\"":{\""layerId\"":\""598e6823-9694-4528-8f4a-c68514e4a613\"",\""accessor\"":\""c45a9309-1774-49e2-b3a9-a77f4f8d9e2d\""},\""query\"":{\""query\"":\""\"",\""language\"":\""kuery\""},\""filters\"":[]},\""references\"":[{\""type\"":\""index-pattern\"",\""id\"":\""background-logs-index-pattern\"",\""name\"":\""indexpattern-datasource-current-indexpattern\""},{\""type\"":\""index-pattern\"",\""id\"":\""background-logs-index-pattern\"",\""name\"":\""indexpattern-datasource-layer-598e6823-9694-4528-8f4a-c68514e4a613\""}]},\""hidePanelTitles\"":false,\""enhancements\"":{}},\""title\"":\""Scale\""}]"",
                ""optionsJSON"": ""{\""hidePanelTitles\"":false,\""useMargins\"":true}"",
                ""version"": 1,
                ""timeRestore"": false,
                ""kibanaSavedObjectMeta"": {
                    ""searchSourceJSON"": ""{\""query\"":{\""language\"":\""kuery\"",\""query\"":\""\""},\""filter\"":[]}""
                }
            },
            ""references"": [
                {
                    ""name"": ""kibanaSavedObjectMeta.searchSourceJSON.index"",
                    ""type"": ""index-pattern"",
                    ""id"": ""background-logs-index-pattern""
                },
                {
                    ""name"": ""kibanaSavedObjectMeta.searchSourceJSON.index"",
                    ""type"": ""index-pattern"",
                    ""id"": ""background-logs-index-pattern""
                },
                {
                    ""type"": ""index-pattern"",
                    ""id"": ""background-logs-index-pattern"",
                    ""name"": ""indexpattern-datasource-current-indexpattern""
                },
                {
                    ""type"": ""index-pattern"",
                    ""id"": ""background-logs-index-pattern"",
                    ""name"": ""indexpattern-datasource-layer-9ef4ec8d-c4c1-4b30-8e58-4f44f1173779""
                },
                {
                    ""type"": ""index-pattern"",
                    ""id"": ""background-logs-index-pattern"",
                    ""name"": ""indexpattern-datasource-current-indexpattern""
                },
                {
                    ""type"": ""index-pattern"",
                    ""id"": ""background-logs-index-pattern"",
                    ""name"": ""indexpattern-datasource-layer-7792e0ba-09e8-4c13-b23b-2a9b04d5f7e7""
                },
                {
                    ""type"": ""index-pattern"",
                    ""id"": ""background-logs-index-pattern"",
                    ""name"": ""indexpattern-datasource-current-indexpattern""
                },
                {
                    ""type"": ""index-pattern"",
                    ""id"": ""background-logs-index-pattern"",
                    ""name"": ""indexpattern-datasource-layer-598e6823-9694-4528-8f4a-c68514e4a613""
                }
            ],
            ""migrationVersion"": {
                ""dashboard"": ""7.11.0""
            },
            ""coreMigrationVersion"": ""7.12.1""
        },
        {
            ""id"": ""background-logs-index-pattern"",
            ""type"": ""index-pattern"",
            ""namespaces"": [
                ""default""
            ],
            ""updated_at"": ""2021-05-20T15:51:59.291Z"",
            ""version"": ""Wzc0MzUsMl0="",
            ""attributes"": {
                ""fieldAttrs"": ""{}"",
                ""title"": ""backgroundlogs-*"",
                ""timeFieldName"": ""endTime"",
                ""fields"": ""[]"",
                ""runtimeFieldMap"": ""{}""
            },
            ""references"": [],
            ""migrationVersion"": {
                ""index-pattern"": ""7.11.0""
            },
            ""coreMigrationVersion"": ""7.12.1""
        }
    ]
}";

            await this.KibanaClient.ImportDashboardAsync(requestJson);
        }
    }
}