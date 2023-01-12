
/************************************* 
Fertigstellung:   <Datum, wann Sie fertig geworden sind> 
Bearbeiter(in) 1:  Vo, Felix, 1347526
Bearbeiter(in) 2:  Hueben, Tobias, 1370737
***************************************/ 

/*
Wir haben C# benutzt. Warum? Ja das würde ich auch gerne wissen.sss
*/

using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;
using HtmlAgilityPack;

namespace bwi40322 {
    class Program {
        static void Main(string[] args)
        {

        /* -------------------------------------------------- 
   Stage 1: Daten vom Webserver holen
-----------------------------------------------------*/ 

WebClient client = new WebClient();
string url = "https://cbrell.de/bwi403/demo/ZaehlerstandExport.csv";

Stream data = client.OpenRead(url);
StreamReader reader = new StreamReader(data);
 
/* -------------------------------------------------- 
   Stage 2: Daten von Lokal laden und aufbereiten 
-----------------------------------------------------*/ 

string sData = reader.ReadToEnd();
File.WriteAllText("ein.csv", sData);

 if(File.Exists("ein.csv")) {
Console.WriteLine("Die Datei ein.csv wurde erstellt.\n");
}
else {
Console.WriteLine("Die Datei ein.csv ist nicht vorhanden.\n");
}

/* -------------------------------------------------- 
   Stage 3: Daten transformieren, Kennzahl(en) erzeugen 
-----------------------------------------------------*/ 
 StreamReader reader2 = new StreamReader("ein.csv");

 string[] int = new string[];
 double[] stromdouble = new double[];
 int zeile = 0;

 while (!reader2.EndOfStream)
 {
   try
   {
      if (isFirstLine != true) {
      string s =reader2.ReadLine();
      string[] values = s.split(';');
      string timestamp = values[0].split(',');
      string strom = values[4];
      
      zeitpunkt[zeile] = Convert.ToInt(timestamp[1]);
      stromdouble[zeile] = Convert.ToDouble(values[4].Replace(',', '.');
      }
      //Wenn isFirstLine true ist, ist die erste Zeile durchlaufenund kann auf false gesetzt werden, um so die erste Zeile zu ueberspringen
      else {
      isFirstLine = false;
      }
      writer.WriteLine(extractedData);
   }
   catch (Exception e)
      {
         
         continue;
      }
   zeile++;
 }
reader2.Close();

int[] zeitpunkt_verdichtet = new string[];
double[] strom_verdichtet = new double[];
int z = 1;
int z2 = 0;

for (int i= 0; i < zeitpunkt.length; i++)
{
   if(i !=0)
   {
      if(zeitpunkt[i] != zeitpunkt[i-1])
      {
         zeitpunkt_verdichtet[z] = zeitpunkt[i];
         strom_verdichtet[z] = stromdouble[i];
      }
   }
   else
   {
      zeitpunkt_verdichtet[i] = zeitpunkt[i];
      strom_verdichtet[i] = stromdouble[i];
   }
}

double[] strommittel = new double[];
for (int x = 0; x < zeitpunkt_verdichtet.length-1; x++)
{
   if(strom_verdichtet[x+1] != 0 && strom_verdichtet[x] != 0)
   {
      strommittel[x] = strom_verdichtet[x+1] - strom_verdichtet[x];
   }
}





 //strommittel @Fel1xVo 
/* -------------------------------------------------- 
   Stage 4: Ausgaben / Visualisierung erzeugen und speichern 
-----------------------------------------------------*/
// Part 1
string htmlFilePath = "index.html";
      string htmlContent = 
              "<!DOCTYPE html>\n" +
              "<html>\n" +
              "<head>\n" +
              "  <meta name=\"viewport\"\n" +
              "    content=\"user-scalable=no, initial-scale=1, maximum-scale=1, minimum-scale=1, width=device-width\">\n" +
              "  <link rel=\"stylesheet\" type=\"text/css\" href=\"styles.css\">\n" +
              "  <title>Chart</title>\n" +
              "</head>\n" +
              "<body>\n" +
              "  <div>\n" +
              "    <div>Bar chart</div>\n" +
              "    <div class=\"bar-chart\">\n" +
              "      <ul class=\"container\" id=\"diagramm1\">\n" +
              "      </ul>\n" +
              "      <div class='x-axis'>\n" +
              "        <div class='hr'></div>\n" +
              "      </div>\n" +
              "      <div class=\"y-axis\"></div>\n" +
              "      <p>Es gibt <span id=\"count\">0</span> Elemente in diesem Diagramm</p>\n" +
              "      <p>Die größte Nummer ist <span id=\"max\">0</span></p>\n" +
              "      <p>Die kleinste Nummer ist <span id=\"min\">0</span></p>\n" +
              "      <script>\n" +
              "        // Elemente Anzahl\n" +
              "        const elementCount = document.getElementsByTagName('li').length;\n" +
              "        document.getElementById('count').innerHTML = elementCount;\n" +
              "\n" +
              "        // x Achse Elemente Style Anpassung\n" +
              "        const elementsX = document.querySelectorAll('.x-axis span');\n" +
              "\n" +
              "        const widthX = (100 / elementCount) + '%';\n" +
              "        for (let element of elementsX) {\n" +
              "          element.style.width = widthX;\n" +
              "        }\n" +
              "\n" +
              "        // Größte und kleinste Zahl herrausfinden\n" +
              "        const div = document.querySelector('.bar-chart');\n" +
              " const listItems = div.querySelectorAll('li');\n" +
              " const numbers = [];\n" +
              "\n" +
              " for (const listItem of listItems) {\n" +
              " const number = listItem.textContent;\n" +
              " numbers.push(number);\n" +
              " }\n" +
              "\n" +
              " numbers.sort((a, b) => b - a);\n" +
              " const largestNumber = numbers[0];\n" +
              " document.getElementById('max').innerHTML = largestNumber;\n" +
              "\n" +
              " numbers.sort((a, b) => a - b);\n" +
              " const smallestNumber = numbers[0];\n" +
              " document.getElementById('min').innerHTML = smallestNumber;\n" +
              "\n" +
              " //Höhe einstellen\n" +
              " for (const item of listItems) {\n" +
              " const number = item.innerHTML;\n" +
              " const heightNumber = (number / largestNumber) * 100;\n" +
              " const style = document.createElement('style');\n" +
              " style.innerHTML = `.bar-chart .container [data-cp-size=\"${ number}\"] { height: ${heightNumber}%; }`;\n" +
              " item.appendChild(style);\n" +
              " }\n" +
              " // Y Labels hinzufügen\n" +
              " const interval = largestNumber / 10;\n" +
              "\n" +
              " for (let i = 10; i >= 0; i--) {\n" +
              " const label = document.createElement('div');\n" +
              " label.classList.add('label');\n" +
              " label.textContent = i * interval;\n" +
              " document.querySelector('.y-axis').appendChild(label);\n" +
              " }\n" +
              "\n" +
              " </script>\n" +
              " </div>\n" +
              "\n" +
              " </div>\n" +
              "</body>\n" +
              "\n" +
              "</html>";
      File.WriteAllText(htmlFilePath, htmlContent);

      string cssFilePath = "styles.css";
      string cssContent =
              ".bar-chart { \n" +
              "  position: relative; \n" +
              "  min-width: 10px; \n" +
              "  min-height: 400px; \n" +
              "  padding: 0; \n" +
              "  margin: 0; \n" +
              "  left: 30px; \n" +
              "  max-width: 98%; \n" +
              "} \n" +
              "\n" +
              ".bar-chart .container { \n" +
              "  display: flex; \n" +
              "  flex-direction: row; \n" +
              "  flex-wrap: nowrap; \n" +
              "  position: absolute; \n" +
              "  text-align: center; \n" +
              "  top: 0; \n" +
              "  left: 0; \n" +
              "  bottom: 0; \n" +
              "  right: 0; \n" +
              "  padding: 0; \n" +
              "  margin: 0; \n" +
              "  min-height: 400px; \n" +
              "} \n" +
              "\n" +
              ".bar-chart .chart-column, \n" +
              ".bar-chart [data-cp-size] { \n" +
              "  flex-grow: 1; \n" +
              "  align-self: flex-end; \n" +
              "} \n" +
              "\n" +
              ".x-axis span { \n" +
              "  cursor: pointer; \n" +
              "  display: inline-block; \n" +
              "  font-size: 12px; \n" +
              "} \n" +
              "\n" +
              ".bar-chart .chart-column, \n" +
              ".bar-chart [data-cp-size] { \n" +
              "  background: #42c2f4; \n" +
              " color: #ffffff; \n" +
              " list-style: none; \n" +
              " border-right: 1px solid #ffffff; \n" +
              " box-sizing: border-box; \n" +
              "} \n" +
              "\n" +
              ".x-axis { \n" +
              " font-size: 0; \n" +
              " position: relative; \n" +
              " top: 400px; \n" +
              "}\n" +
              ".hr { \n" +
              " height: 2px; \n" +
              " background-color: #000000; \n" +
              "} \n" +
              "\n" +
              ".bar-chart { \n" +
              " border-left: 2px solid #000000; \n" +
              "} \n" +
              "\n" +
              ".x-axis { \n" +
              " width: 100%; \n" +
              "} \n" +
              "\n" +
              ".x-axis span { \n" +
              " text-align: center; \n" +
              "} \n" +
              "\n" +
              ".bar-chart .y-axis { \n" +
              " position: absolute; \n" +
              " left: -35px; \n" +
              " top: 0; \n" +
              " bottom: 0; \n" +
              " width: 35px; \n" +
              " display: block; \n" +
              "} \n" +
              "\n" +
              ".bar-chart .y-axis .label { \n" +
              " position: relative; \n" +
              " left: 0; \n" +
              " font-size: 12px; \n" +
              " font-weight: bold; \n" +
              " height: 39px; \n" +
              " border-top: 1px solid black; \n" +
              "} \n" +
              "\n" +
              "p { \n" +
              " position: relative; \n" +
              " top: 420px; \n" +
              "}";
      File.WriteAllText(cssFilePath, cssContent);
// Part 2
int[,] strommittel = new int[10, 2] { { 20, 1 }, { 40, 2 }, { 60, 3 }, { 80, 4 }, { 100, 5 }, { 120, 6 },
      { 140, 7 }, { 160, 8 }, { 180, 9 }, { 200, 10 } };

      HtmlDocument htmlDoc = new HtmlDocument();

      // Lade die HTML-Datei
      htmlDoc.Load("index.html");

      // Suche das ul-Element mit der Klasse "container"
      HtmlNode ulNode = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='container']");

      if (ulNode != null)
      {
        for (int i = 0; i < strommittel.GetLength(0); i++)
        {
          // Erstelle ein neues li-Element mit dem Wert "300" und dem data-cp-size Attribut
          HtmlNode liNode = HtmlNode.CreateNode("<li data-cp-size='" + strommittel[i, 0] + "'>" + strommittel[i, 0] + "</li>");

          // Füge das li-Element als letztes Kind des ul-Elements hinzu
          ulNode.AppendChild(liNode);
   }

        }

      // Suche das div-Element mit der Klasse "x-axis"
      HtmlNode divNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='x-axis']");

      if (divNode != null)
      {

        for (int i = 0; i < strommittel.GetLength(0); i++)
        {
          // Erstelle ein neues span-Element mit dem Wert "10"
          HtmlNode spanNode = HtmlNode.CreateNode("<span>" + strommittel[i, 1] + "</span>");

          // Füge das span-Element als letztes Kind des div-Elements hinzu
          divNode.AppendChild(spanNode);
        }
}
      // Speichere die HTML-Datei
      htmlDoc.Save("index.html");
        }
        }
}

