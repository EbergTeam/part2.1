using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1
{
    public class Startup
    {
        // îáðàáîòêà çàïðîñîâ ìåòîäàìè îáúåêòà IApplicationBuilder ïî ïðèíöèïó êîíâåéåðà. Êîìïîíåíòû êîíâåéåðà - middleware
        // äëÿ êîíôèãóðàöèè êîíâåéåðà îáðàáîòêè çàïðîñà ïðèìåíÿþòñÿ ìåòîäû Run, Map è Use
        public void Configure(IApplicationBuilder app)
        {
            // USE - äîáàâëåíèå êîìïîíåíòà middleware â êîíâåéåð, îïðåäåëåííûé êàê àíîíèìíûé ìåòîä
            // ìîæåò ïåðåäàòü óïðàâëåíèå ñëåä. êîìïîíåíòó ÷åðåç await next.Invoke(), íî ïîñëå âûïîëíèòñÿ êîä ïîñëå await next.Invoke()
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("USE BEFORE Invoke() \n");
                await next.Invoke();
                await context.Response.WriteAsync("\nUSE AFTER Invoke()");
            });

            // MAP àíàëèçèðóåò ïóòü çàïðîñà. Åñëè çàïðîñ âèäà //localhost:44322/about/, òî âûïîëíÿåòñÿ ìåòîä About
            app.Map("/about", About);
            // MapWhen(true/false, About); Åñëè óñëîâèå èñòèííî, òî âûçûâàåì ìåòîä About()
            
            // Âëîæåííûå ìåòîäû MAP - àíàëèçèðóåò ïóòü çàïðîñà. Â çàâèñèìîñòè îò ïóòè âûçûâàåì íóæíûé...
            app.Map("/index", (appBuilder) =>
            {
                // ..ìåòîä
                appBuilder.Map("/help", Help);

                //  ..àíîíèìíûé ìåòîä
                appBuilder.Run(async (context) =>
                {
                    await context.Response.WriteAsync("INDEX");
                });
            });

            // RUN - äîáàâëåíèå êîìïîíåíòà middleware â êîíâåéåð, îïðåäåëåííûé êàê àíîíèìíûé ìåòîä 
            // íå âûçûâàåò íèêàêèå äðóãèå êîìïîíåíòû è äàëüøå îáðàáîòêó çàïðîñà íå ïåðåäàåò (ÿâëÿåòñÿ òåðìèíàëüíûì)
            app.Run(async (context) =>
            {
                var host = context.Request.Host.Value;
                var path = context.Request.Path;
                var query = context.Request.QueryString.Value;
                await context.Response.WriteAsync("RUN host=" + host + ", path " + path + ", query" + query);              
            });
        }

        private static void About(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Run(async (context) =>
            {
                await context.Response.WriteAsync("ABOUT");
            });
        }

        private static void Help(IApplicationBuilder applicationBuilder)
        {
            applicationBuilder.Run(async (context) =>
            {
                await context.Response.WriteAsync("HELP");
            });
        }
    }
}
