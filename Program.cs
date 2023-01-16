
/************************************* 
Fertigstellung:   16.01.2023
Bearbeiter(in) 1:  Vo, Felix, 1347526
Bearbeiter(in) 2:  Hueben, Tobias, 1370737
***************************************/

/*
Wir haben C# für unser Projekt benutzt. Dies ist auf den trivialen Grund zurueckzufuehren, 
dass wir uns bisher haeufiger mit C# als mit Python beschaeftig haben und somit sicherer im Umgang waren.
*/

using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using System.Drawing;

namespace bwi40322
{
  class Program
  {
    static void Main(string[] args)
    {

      /* -------------------------------------------------- 
      Stage 1: Daten vom Webserver holen
      -----------------------------------------------------*/
      //Konsolenausgabe
      Console.WriteLine("-------Programm wrid gestartet\n-------");

      WebClient client = new WebClient();
      string url = "https://cbrell.de/bwi403/demo/ZaehlerstandExport.csv";

      //Konsolenausgabe, um den Nutzer ueber den aktuellen Status zu informieren
      Console.WriteLine("Die Daten werden vom Webservice geladen...\n");

      Stream data = client.OpenRead(url);
      // Es wird ein StreamReader geoeffnet um die Daten einzulesen.
      StreamReader reader = new StreamReader(data);

      /* -------------------------------------------------- 
         Stage 2: Daten von Lokal laden und aufbereiten 
      -----------------------------------------------------*/

      string sData = reader.ReadToEnd();
      // Die eingelesenen Daten werden in die Datei "ein.csv" geschrieben


      File.WriteAllText("ein.csv", sData);

      // Es wird überprueft, ob die Datei erfolgreich erstellt wurde
      if (File.Exists("ein.csv"))
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

      // Es wird ein StreamReader erstellt, der "ein.csv" einlesen soll
      StreamReader reader2 = new StreamReader("ein.csv");

      //Liste, die Zeitstempel speichert
      List<string> zeitpunkt = new List<string>();
      //Liste, die die Zaehlerstände des Stroms speichert
      List<double> stromdouble = new List<double>();
      Boolean isFirstLine = true;
      //Der Datenstrom wird vollständig durchlaufen
      while (!reader2.EndOfStream)
      {
        //die erste Zeile wird uebersprungen, da dort keine (nutzbaren) Werte vorhanden sind
        if (isFirstLine != true)
        {
          //Die ausgelesene Zeile wird als String zwischengespeichert
          string s = reader2.ReadLine();
          //Die duch Kommata getrennten Werte der csv-Datei werden aufgespalten.
          string[] values = s.Split(';');
          //Der zeitstempel wird auf den Tag eingekürzt
          string[] timestamp = values[1].Split(',');

          if (String.IsNullOrEmpty(values[3])) { }
          else
          {
            zeitpunkt.Add(timestamp[0]);
            //Der Wert des Stromzaehlers wird zur weiteren verwendung umformatiert.
            stromdouble.Add(Convert.ToDouble(values[3]));
          }
        }
        //Wenn isFirstLine true ist, ist die erste Zeile durchlaufenund kann auf false gesetzt werden, um so die erste Zeile zu ueberspringen
        else
        {
          reader2.ReadLine();
          isFirstLine = false;
        }
      }
      reader2.Close();
      //Liste, die Zeitstempel speichert
      List<string> zeitpunkt_verdichtet = new List<string>();
      //Liste, die Stromzaehlerstände speichert
      List<double> strom_verdichtet = new List<double>();

      //Die Liste der Zeitpunkte wird durchlaufen
      for (int i = 0; i < zeitpunkt.Count; i++)
      {
        if (i != 0)
        {
          //Es wird ueberprueft, ob für den Zeitstempel (Tag) schon ein Eintrag vorhanden ist
          if (!zeitpunkt[i].Equals(zeitpunkt[i - 1]))
          {
            zeitpunkt_verdichtet.Add(zeitpunkt[i]);
            strom_verdichtet.Add(stromdouble[i]);
          }
        }
        // Der Vergleich der Daten entfaellt für die erste zeile
        else
        {
          zeitpunkt_verdichtet.Add(zeitpunkt[i]);
          strom_verdichtet.Add(stromdouble[i]);
        }
      }
      Console.WriteLine("Daten verdichtet.");

      //Array, das Zeitstempel und Stromverbrauch zusammenführt
      string[,] strommittel = new string[zeitpunkt_verdichtet.Count - 1, 2];
      double berechnung = 0;
      for (int x = 0; x < zeitpunkt_verdichtet.Count - 1; x++)
      {
        //Zeitstempel wird in die erste Spalte geschrieben
        strommittel[x, 0] = zeitpunkt_verdichtet[x];

        //Stromverbrauch wird aus aktuellem Zaehlerstand und nachfolgendem Eintrag berechnet
        berechnung = strom_verdichtet[x + 1] - strom_verdichtet[x];
        //Stromverbrauch wird in die zweite Spalte geschrieben
        strommittel[x, 1] = berechnung.ToString();

      }

      Console.WriteLine("Datenpaare gebildet.");

      /* -------------------------------------------------- 
         Stage 4: Ausgaben / Visualisierung erzeugen und speichern 
      -----------------------------------------------------*/

      //Die ermittelten Stromverbräuche werden nun in "kennzahlen.txt" geschrieben.
      StreamWriter writer = new StreamWriter("kennzahlen.txt");
      writer.WriteLine("Zeitstempel, Stromverbrauch");
      for (int i = 0; i < strommittel.GetLength(0); i++)
      {
        string zeile = strommittel[i, 0] + ", " + strommittel[i, 1];
        writer.WriteLine(zeile);
      }
      writer.Close();

      Console.WriteLine("Kennzahlen.txt erstellt.")

      // Ein String mit den Beschriftungen der Tabelle sowie der Tabellennummer wird erzeugt
      string[] strommitteldaten = new string[4] {
            "1",
            "Mittelwert des Stromverbrauchs",
            "Datum",
            "Stromverbrauch kwh"
         };

      // Name der HTML Datei
      string htmlFilePath = "index.html";
      // Dieser String beinhaltet die komplette HTML Datei
      string htmlContent =
         // Hier faengt die Datei an
         "<!DOCTYPE html>\n" +
         "<html>\n" +
         // Dateikopf
         "<head>\n" +
         "<meta name=\"viewport\"\n" +
         "content=\"user-scalable=no, initial-scale=1, maximum-scale=1, minimum-scale=1, width=device-width\">\n" +
         "<link rel=\"stylesheet\" type=\"text/css\" href=\"styles.css\">\n" +
         "<title>Mittlerer Stromverbauch pro Tag</title>\n" +
         "</head>\n" +
         // Ab hier fängt die Hauptkomponente der HTML datei an
         "<body>\n" +
         AusgabeTabelle(strommittel, strommitteldaten) +
         "<script>" +
         "ElementeAnzahl('1'); \n" +
         "function ElementeAnzahl(nummer) {\n" +

         // Hier wird nach der Gesamtanzahl an Listenobjekten gesucht und ausgegeben
         "const elementCount = document.querySelectorAll('#diagramm' + nummer + ' li').length;\n" +
         "document.getElementById('count' + nummer).innerHTML = elementCount;\n" +

         // Es wird die kleinste und groeßte Zahl herrausgefunden und ausgegeben
         "const listItems = document.querySelectorAll('#diagramm' + nummer + ' li')\n" +
         "const numbers = [];\n" +
         "for (const listItem of listItems) {\n" +
         "  const number = listItem.dataset.cpSize;\n" +
         "  numbers.push(number);\n" +
         "}\n" +

         // Hier wird nach der groeßten Nummer gesucht
         "numbers.sort((a, b) => b - a);\n" +
         "const largestNumber = numbers[0];\n" +
         "document.getElementById('max' + nummer).innerHTML = largestNumber;\n" +

         // Hier wird nach der kleinsten Nummer gesucht
         "numbers.sort((a, b) => a - b);\n" +
         "const smallestNumber = numbers[0];\n" +
         "document.getElementById('min' + nummer).innerHTML = smallestNumber;\n" +

         // Die Hoehe der einzelnen Saeulen wird hier brechnet und zugewiesen
         "for (const item of listItems) {\n" +
         "  const number = item.dataset.cpSize;\n" +
         "  const heightNumber = (number / largestNumber) * 100;\n" +
         "  const style = document.createElement('style');\n" +
         "  style.innerHTML = `#${'diagramm' + nummer}.content [data-cp-size=\"${number}\"] { height: ${heightNumber}%; }`;\n" +
         "  item.appendChild(style);\n" +
         "}\n" +

         // Der Abstand zwischen den einzelnen X-Achsenbeschriftungunge wird hier berechnet und zugewiesen
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

         // Die Zahlen für die Y-Achse werden hier berechnet und zugewiesen
         "const interval = largestNumber / 10;\n" +
         "for (let i = 10; i >= 1; i--) {\n" +
         "const label = document.createElement('div');\n" +
         "label.classList.add('label');\n" +
         "label.textContent = Math.floor(i * interval);\n" +
         "document.querySelector('#diagramm' + nummer + '.y-axis').appendChild(label);\n" +
         "}\n" +
         "}\n" +
         "</script>\n" +
         "</html>\n";

      //Konsolenausgabe, um den Nutzer ueber den aktuellen Status zu informieren
      Console.WriteLine("Die Datei Namens index.html wurde erfolgreich erstellt");

      // Die HTML-Datei wird erstellt
      File.WriteAllText(htmlFilePath, htmlContent);

      // Der Name der CSS Datei
      string cssFilePath = "styles.css";

      // Dieser String enthaelt die gesamte CSS Datei
      string cssContent =
         // Hier werden die Sauelen definiert
         ".bar-chart { position: relative; min-width: 10px; min-height: 400px; padding: 0; margin: 0; left: 40px; max-width: 800px; border-left: 2px solid #000000;} \n" +
         ".bar-chart .content { display: flex; flex-direction: row; flex-wrap: nowrap; position: absolute; text-align: center; top: 0; left: 0; bottom: 0; right: 0; padding: 0; margin: 0; min-height: 400px; }\n" +
         ".bar-chart .chart-column, .bar-chart [data-cp-size] { flex-grow: 1; align-self: flex-end; min-width: 10px; background: #FFBF00; list-style: none; border-right: 1px solid #ffffff; box-sizing: border-box; }\n" +
         // Hier wird die x-Achse definiert
         ".x-axis span { cursor: pointer; display: inline-block; font-size: 12px; height: 10px; transform: rotate(90deg); text-align: center; margin-top: 4px; }\n" +
         ".x-axis { font-size: 0; position: relative; top: 400px; width: 100%;}\n" +
         ".beschriftung-x { position: relative; left: 800px; top: 380px; height: 2px; }\n" +
         // Hier wird die Y-Achse definiert
         ".bar-chart .y-axis { position: absolute; left: -40px; top: 0; width: 40px; display: block; height: 400px; }\n" +
         ".bar-chart .y-axis .label { font-size: 10px; box-sizing: border-box; position: relative; left: 0; font-weight: bold; height: 40px; border-top: 1px solid black; border-bottom: 1px solid black; }\n" +
         // Hier werden die Beschriftungen definiert
         ".hr { height: 2px; background-color: #000000; }\n" +
         "p { position: relative; top: 420px; }\n" +
         "h5 { position: relative; }\n" +
         // Hier wird der Abstand zur naechsten Grafik definiert
         ".abstand { height: 180px; border-bottom: 1px solid #000000; }\n";
      // Die CSS-Datei wird erstellt   
      File.WriteAllText(cssFilePath, cssContent);

      //Konsolenausgabe, um den Nutzer ueber den aktuellen Status zu informieren
      Console.WriteLine("Die Datei Namens styles.html wurde erfolgreich erstellt");

      // Diese Methode fuegt dem HTML Dokument die Zahlendaten hinzu
      string AusgabeDatenwert(string[,] strommittel)
      {
        string datenwert = "";
        // Diese Schleife geht durch Alle Zahlendaten durch
        for (int i = 0; i < strommittel.GetLength(0); i++)
        {
          // Dem HTML Dokument werden die Zahlendaten in Form eines Listenelements uebergeben. Es wird von , auf . formatiert da Javascript nachkommastellen mit . benoetigt
          datenwert = datenwert + "<li data-cp-size='" + strommittel[i, 1].Replace(",", ".") + "'>" + "</li> \n";
        }

        return datenwert;
      }

      // Diese Methode fügt dem HTML Dokument die Daten für die X-Achsenbeschriftung hinzu
      string AusgabeUeberschrift(string[,] strommittel)
      {
        string ueberschrift = "";
        for (int i = 0; i < strommittel.GetLength(0); i++)
        {
          // Konvertieren des Zeitstempels in einen int-Wert
          int timestampInt = int.Parse(strommittel[i, 0]);

          // Erstellen eines neuen DateTime-Objekts mit dem Zeitstempelwert + 1. Januar 1900
          DateTime date = new DateTime(1899, 12, 30).AddDays(timestampInt);

          // Formatieren des Datums im gewünschten Format
          string formattedDate = date.ToString("dd.M.yy");

          ueberschrift = ueberschrift + "<span>" + formattedDate + "</span> \n";
        }
        return ueberschrift;
      }

      // Diese Methode erstellt ein Grundgeruest von einem Balkendiagramm und fügt diesen der HTML Datei hinzu 
      string AusgabeTabelle(string[,] daten, string[] datenname)
      {
        string ausgabe =
           "  <div>\n" +
           // Wichtige Beschriftungen werden hinzugefuegt
           "    <div class=\"ueberschrift\" id=\"diagramm" + datenname[0] + "\">" + datenname[1] + "</div>\n" +
           "    <div class=\"beschriftung-y\" id=\"diagramm" + datenname[0] + "\">" + datenname[3] + "</div>\n" +
           "    <div class=\"bar-chart\">\n" +
           "      <ul class=\"content\" id=\"diagramm" + datenname[0] + "\">\n" +
           // Die Methode wird aufgerufen, um die Zahlendaten dem Konstrukt hinzuzufuegen
           AusgabeDatenwert(daten) +
           " </ul>\n" +
           " <div class='x-axis' id='diagramm" + datenname[0] + "'>\n" +
           " <div class='hr' id='diagramm" + datenname[0] + "'></div>\n" +
           // Die Methode wrd aufgerufen, um die Beschriftungsdatem dem Konstrukt hinzuzufuegen
           AusgabeUeberschrift(daten) +
           " </div>\n" +
           // Beschriftungen der x Achse
           " <span class='beschriftung-x' id='diagramm" + datenname[0] + "'>" + datenname[2] + "</span>\n" +
           " <div class='y-axis' id='diagramm" + datenname[0] + "'></div>\n" +
           // Hier werden die wichtigsten Daten des Diagramms genannt
           "<p>Es gibt <span id=\"count" + datenname[0] + "\">0</span> Elemente in diesem Diagramm</p>\n" +
           " <p>Die größte Nummer ist <span id='max" + datenname[0] + "'>0</span></p>\n" +
           " <p>Die kleinste Nummer ist <span id='min" + datenname[0] + "'>0</span></p>\n" +
           " </div>\n" +
           " </div>\n" +
           "\n" +
           // Dieses Div-Element schafft Abstand zum nächstem Diagramm
           " <div class='abstand'></div>\n";
        return ausgabe;
      }
    }
  }
}
