﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerAPIs
{
    public static class HealthChecksResponseWriter
    {
        public static Task WriteUIResponse(HttpContext httpContext, HealthReport report)
        {
            var groups = report.Entries.GroupBy(x => x.Value.Tags.FirstOrDefault());

            var result = new Dictionary<string, IReadOnlyDictionary<string, object>>();

            foreach (var group in groups)
            {
                var list = new Dictionary<string, object>();

                foreach (var (_, entry) in group)
                {
                    list.Add(entry.Description!, entry.Data);
                }

                result.Add(group.Key, list);
            }

            var content = JsonSerializer.Serialize(result, new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            });

            httpContext.Response.ContentType = MediaTypeNames.Application.Json;

            return httpContext.Response.WriteAsync(content);
        }
    }
}
