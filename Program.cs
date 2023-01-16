
/************************************* 
Fertigstellung:   16.01.2023
Bearbeiter(in) 1:  Vo, Felix, 1347526
Bearbeiter(in) 2:  Hueben, Tobias, 1370737
***************************************/ 

/*
Wir haben C# für unser Projekt benutzt. Dies ist auf den trivialen Grund zurückzuführen, 
dass wir uns bisher häufiger mit C# als mit Python beschäftig haben und somit sicherer im Umgang waren.
*/

using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;

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
StreamReader reader = new StreamReader(data); //es wird ein StreamReader geöffnet um die Daten einzulesen.
 
/* -------------------------------------------------- 
   Stage 2: Daten von Lokal laden und aufbereiten 
-----------------------------------------------------*/ 

string sData = reader.ReadToEnd();
File.WriteAllText("ein.csv", sData); //die eingelesenen Daten werden in die Datei "ein.csv" geschrieben

if(File.Exists("ein.csv")) //es wird überprüft, ob die Datei erfolgreich erstellt wurde
{
   Console.WriteLine("Datei erfolgreich erstellt.\n");
}
else 
{
   Console.WriteLine("Datei nicht vorhanden.\n");
}

/* -------------------------------------------------- 
   Stage 3: Daten transformieren, Kennzahl(en) erzeugen 
-----------------------------------------------------*/ 
 StreamReader reader2 = new StreamReader("ein.csv"); //Es wird ein StreamReader erstellt, der "ein.csv" einlesen soll


 List<string> zeitpunkt = new List<string>(); //Liste, die Zeitstempel speichert
 List<double> stromdouble = new List<double>(); //Liste, die die Zählerstände des Stroms speichert
 Boolean isFirstLine = true;

 while (!reader2.EndOfStream) //Der Datenstrom wird vollständig durchlaufen
 {
   try
   {
      if (isFirstLine != true) { //die erste Zeile wird übersprungen, da dort keine (nutzbaren) Werte vorhanden sind
      string s = reader2.ReadLine(); //Die ausgelesene Zeile wird als String zwischengespeichert
      string[] values = s.Split(';'); //Die duch Kommata getrennten Werte der csv-Datei werden aufgespalten.
      string[] timestamp = values[0].Split(','); //Der zeitstempel wird auf den Tag eingekürzt
      
      zeitpunkt.Add(timestamp[0]);
      stromdouble.Add(Convert.ToDouble(values[4].Replace(',', '.'))); //Der Wert des Stromzählers wird zur weiteren verwendung umformatiert.
      }
      //Wenn isFirstLine true ist, ist die erste Zeile durchlaufenund kann auf false gesetzt werden, um so die erste Zeile zu ueberspringen
      else {
      isFirstLine = false;
      }
   }
   catch (Exception e)
      {
         
         continue;
      }
 }
reader2.Close();

List<string> zeitpunkt_verdichtet = new List<string>(); //Liste, die Zeitstempel speichert
List<double> strom_verdichtet = new List<double>(); //Liste, die Stromzählerstände speichert

for (int i= 0; i < zeitpunkt.Count; i++) //Die Liste der Zeitpunkte wird durchlaufen
{
   if(i !=0)
   {
      if(!zeitpunkt[i].Equals(zeitpunkt[i-1])) //Es wird überprüft, ob für den Zeitstempel (Tag) schon ein Eintrag vorhanden ist
      {
        zeitpunkt_verdichtet.Add(zeitpunkt[i]);
        strom_verdichtet.Add(stromdouble[i]);
      }
   }
   else // Der Vergleich der Daten entfällt für die erste zeile
   {
        zeitpunkt_verdichtet.Add(zeitpunkt[i]);
        strom_verdichtet.Add(stromdouble[i]);
   }
}

string[,] strommittel = new string[zeitpunkt_verdichtet.Count, 2]; //Array, das Zeitstempel und Stromverbrauch zusammenführt
double berechnung = 0;
for (int x = 0; x < zeitpunkt_verdichtet.Count-1; x++)
{
   strommittel[x, 0] = zeitpunkt_verdichtet[x]; //Zeitstempel wird in die erste Spalte geschrieben
   if(strom_verdichtet[x+1] != 0 && strom_verdichtet[x] != 0) //Es wird überprüft, ob einer der beiden Werte 0 ist.
   {
      berechnung = strom_verdichtet[x+1] - strom_verdichtet[x]; //Stromverbrauch wird aus aktuellem Zählerstand und nachfolgendem Eintrag berechnet
      strommittel[x, 1] = berechnung.ToString(); //Stromverbrauch wird in die zweite Spalte geschrieben
   }
   else
   {
      strommittel[x, 1] = "0"; //Ist einer der beiden Operanden 0, wird 0 als Stromverbrauch angenommen
   }
}

//Die ermittelten Stromverbräuche werden nun in "kennzahlen.txt" geschrieben.
StreamWriter writer = new StreamWriter("kennzahlen.txt") 
writer.WriteLine("Zeitstempel, Stromverbrauch");
for(int i = 0; i<strommittel.length; i++)
{
   string zeile = strommittel[i][0] + strommittel[i][1];
   writer.WriteLine(zeile);
}




 //strommittel @Fel1xVo 
/* -------------------------------------------------- 
   Stage 4: Ausgaben / Visualisierung erzeugen und speichern 
-----------------------------------------------------*/
// Part 1
      //int[,] strommittel = new int[10, 2] { { 20, 1 }, { 40, 2 }, { 60, 3 }, { 80, 4 }, { 100, 5 }, { 120, 6 },
     // { 140, 7 }, { 160, 8 }, { 180, 9 }, { 200, 10 } };
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
              AusgabeDatenwert(strommittel) +
              "      </ul>\n" +
              "      <div class='x-axis'>\n" +
              "        <div class='hr'></div>\n" +
              AusgabeUeberschrift(strommittel) +
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
      string AusgabeDatenwert(int[,] strommittel)
      {
        string datenwert = "";
        for (int i = 0; i < strommittel.GetLength(0); i++)
        {
          datenwert = datenwert + "<li data-cp-size='" + strommittel[i, 0] + "'>" + strommittel[i, 0] + "</li> \n";
        }

        return datenwert;
      }

      string AusgabeUeberschrift(int[,] strommittel)
      {
        string ueberschrift = "";
        for (int i = 0; i < strommittel.GetLength(0); i++)
        {
          ueberschrift = ueberschrift + "<span>" + strommittel[i, 1] + "</span> \n";
        }
        return ueberschrift;
      }
    }
        }
}

