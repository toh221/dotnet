
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

string[,] strommittel = new string[zeitpunkt_verdichtet.Count-1, 2]; //Array, das Zeitstempel und Stromverbrauch zusammenführt
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
      string[] strommitteldaten = new string[4] { "1", "Mittelwert des Stromverbrauchs", "Datum", "Stromverbrauch kwh" };

      string htmlFilePath = "index.html";
      string htmlContent =
              "<!DOCTYPE html>\n" +
              "<html>\n" +
              "<head>\n" +
              "<meta name=\"viewport\"\n" +
              "content=\"user-scalable=no, initial-scale=1, maximum-scale=1, minimum-scale=1, width=device-width\">\n" +
              "<link rel=\"stylesheet\" type=\"text/css\" href=\"styles.css\">\n" +
              "<title>Chart</title>\n" +
              "</head>\n" +
              "<body>\n" +
              AusgabeTabelle(strommittel, strommitteldaten) +
    "<script>" +
      "ElementeAnzahl('1'); \n" +
      "function ElementeAnzahl(nummer) {\n" +
      "// Elemente Anzahl\n" +
      "const elementCount = document.querySelectorAll('#diagramm' + nummer + ' li').length;\n" +
      "document.getElementById('count' + nummer).innerHTML = elementCount;\n" +
      "// Größte und kleinste Zahl herrausfinden\n" +
      "const listItems = document.querySelectorAll('#diagramm' + nummer + ' li')\n" +
      "const numbers = [];\n" +
      "for (const listItem of listItems) {\n" +
      "  const number = listItem.dataset.cpSize;\n" +
      "  numbers.push(number);\n" +
      "}\n" +
      "numbers.sort((a, b) => b - a);\n" +
      "const largestNumber = numbers[0];\n" +
      "document.getElementById('max' + nummer).innerHTML = largestNumber;\n" +

      "numbers.sort((a, b) => a - b);\n" +
      "const smallestNumber = numbers[0];\n" +
      "document.getElementById('min' + nummer).innerHTML = smallestNumber;\n" +

      "//Höhe einstellen\n" +
      "for (const item of listItems) {\n" +
      "  const number = item.dataset.cpSize;\n" +
      "  const heightNumber = (number / largestNumber) * 100;\n" +
      "  const style = document.createElement('style');\n" +
      "  style.innerHTML = `#${'diagramm' + nummer}.content [data-cp-size=\"${number}\"] { height: ${heightNumber}%; }`;\n" +
      "  item.appendChild(style);\n" +
      "}\n" +

      "// x Achse Elemente Style Anpassung\n" +
      "const elementsX = document.querySelectorAll('#diagramm' + nummer + ' span');\n" +
      "const elements = document.querySelectorAll('#diagramm' + nummer + ' li');\n" +
      "var widthElements = 0;\n" +
      "for (var x = 0; x < elements.length; x++) {\n" +
      "  widthElements = widthElements + elements[x].clientWidth;\n" +
      "}\n" +
      "for (var i = 0; i < elementsX.length; i++) {\n" +
      "  var width = elements[i].clientWidth;\n" +
      "  elementsX[i].style.width = (width / widthElements) * 100 + \"%\";\n" +
      "}" +

      "// Y Labels hinzufügen\n" +
      "const interval = largestNumber / 10;\n" +

      "for (let i = 10; i >= 1; i--) {\n" +
      "  const label = document.createElement('div');\n" +
      "  label.classList.add('label');\n" +
      "  label.textContent = Math.floor(i * interval);\n" +
      "  document.querySelector('#diagramm' + nummer + '.y-axis').appendChild(label);\n" +
    "  }\n" +
    "}\n" +
    "</script>\n" +
    "</html>\n";
      File.WriteAllText(htmlFilePath, htmlContent);
      string cssFilePath = "styles.css";
      string cssContent =
      ".bar-chart { position: relative; min-width: 10px; min-height: 400px; padding: 0; margin: 0; left: 40px; max-width: 800px; } \n" +
      ".bar-chart .content { display: flex; flex-direction: row; flex-wrap: nowrap; position: absolute; text-align: center; top: 0; left: 0; bottom: 0; right: 0; padding: 0; margin: 0; min-height: 400px; }\n" +
      ".bar-chart .chart-column, .bar-chart [data-cp-size] { flex-grow: 1; align-self: flex-end; }\n" +
      ".x-axis span { cursor: pointer; display: inline-block; font-size: 12px; height: 10px; }\n" +
      ".bar-chart .chart-column, .bar-chart [data-cp-size] { min-width: 10px; background: #FFBF00; list-style: none; border-right: 1px solid #ffffff; box-sizing: border-box; }\n" +
      ".x-axis { font-size: 0; position: relative; top: 400px; }\n" +
      ".hr { height: 2px; background-color: #000000; }\n" +
      ".bar-chart { border-left: 2px solid #000000; }\n" +
      ".x-axis { width: 100%; }\n" +
      ".x-axis span { transform: rotate(90deg); text-align: center; margin-top: 4px;}\n" +
      ".bar-chart .y-axis { position: absolute; left: -40px; top: 0; width: 40px; display: block; height: 400px; }\n" +
      ".bar-chart .y-axis .label { font-size: 10px; box-sizing: border-box; position: relative; left: 0; font-weight: bold; height: 40px; border-top: 1px solid black; border-bottom: 1px solid black; }\n" +
      "p { position: relative; top: 420px; }\n" +
      "h5 { position: relative; }\n" +
      ".beschriftung-x { position: relative; left: 800px; top: 380px; height: 2px; }\n" +
      ".abstand { height: 180px; border-bottom: 1px solid #000000; }\n";
      File.WriteAllText(cssFilePath, cssContent);
      Process.Start("C:/Users/Felix/Desktop/app/index.html");
      // Part 2
      string AusgabeDatenwert(string[,] strommittel)
      {
        string datenwert = "";
        for (int i = 0; i < strommittel.GetLength(0); i++)
        {
          datenwert = datenwert + "<li data-cp-size='" + strommittel[i, 0] + "'>" + "</li> \n";
        }

        return datenwert;
      }

      string AusgabeUeberschrift(string[,] strommittel)
      {
        string ueberschrift = "";
        for (int i = 0; i < strommittel.GetLength(0); i++)
        {
          ueberschrift = ueberschrift + "<span>" + strommittel[i, 1] + "</span> \n";
        }
        return ueberschrift;
      }

      string AusgabeTabelle(string[,] daten, string[] datenname)
      {
        string ausgabe =
                "  <div>\n" +
                "    <div class=\"ueberschrift\" id=\"diagramm" + datenname[0] + "\">" + datenname[1] + "</div>\n" +
                "    <div class=\"beschriftung-y\" id=\"diagramm" + datenname[0] + "\">" + datenname[3] + "</div>\n" +
                "    <div class=\"bar-chart\">\n" +
                "      <ul class=\"content\" id=\"diagramm" + datenname[0] + "\">\n" +
                AusgabeDatenwert(daten) +
                " </ul>\n" +
                " <div class='x-axis' id='diagramm" + datenname[0] + "'>\n" +
                " <div class='hr' id='diagramm" + datenname[0] + "'></div>\n" +
                AusgabeUeberschrift(daten) +
                " </div>\n" +
                " <span class='beschriftung-x' id='diagramm" + datenname[0] + "'>" + datenname[2] + "</span>\n" +
                " <div class='y-axis' id='diagramm" + datenname[0] + "'></div>\n" +
                "<p>Es gibt <span id=\"count" + datenname[0] + "\">0</span> Elemente in diesem Diagramm</p>\n" +
                " <p>Die größte Nummer ist <span id='max" + datenname[0] + "'>0</span></p>\n" +
                " <p>Die kleinste Nummer ist <span id='min" + datenname[0] + "'>0</span></p>\n" +
                " </div>\n" +
                " </div>\n" +
                "\n" +
                " <div class='abstand'></div>\n";
        return ausgabe;
      }
        }
}

