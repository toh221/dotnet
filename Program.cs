
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
 StreamWriter writer = new StreamWriter("aus-Sahler.csv");

 List<string> zeitpunkt = new List<string> ();
 List<string> stromkwh = new List<string> ();

 while (!reader2.EndOfStream)
 {
   try
   {
      
   
   
   string s =reader2.ReadLine();
   string[] values = s.split(';');
   string timestamp = values[0];
   string strom = values[4];
   string extractedData = timestamp + ";" + strom;
   if (isFirstLine != true) {
//Der Zeitstempel und die Temperatur werden in derjeweiligen Liste zwischengespeichert
zeitpunkt.Add(timestamp);
stromkwh.Add(strom);
}
//Wenn isFirstLine true ist, ist die erste Zeile durchlaufen und kann auf false gesetzt werden, um so die erste Zeile zu ueberspringen
else {
isFirstLine = false;
}
writer.WriteLine(extractedData);
 }
catch (Exception e)
   {
      
      continue;
   }
 }
reader2.Close();
writer.Close();

 //strommittel @Fel1xVo 
/* -------------------------------------------------- 
   Stage 4: Ausgaben / Visualisierung erzeugen und speichern 
-----------------------------------------------------*/
HtmlDocument htmlDoc = new HtmlDocument();

int[] strommittel = new int[10,2];

// Lade die HTML-Datei
htmlDoc.Load("index.html");

// Suche das ul-Element mit der Klasse "container"
HtmlNode ulNode = htmlDoc.DocumentNode.SelectSingleNode("//ul[@class='container']");

if (ulNode != null)
{  
   foreach(int daten in strommittel)
    // Erstelle ein neues li-Element mit dem Wert "300" und dem data-cp-size Attribut
    HtmlNode liNode = HtmlNode.CreateNode("<li data-cp-size='" +strommittel[daten,1]+ "'>" + strommitel[daten,1] + "</li>");

    // Füge das li-Element als letztes Kind des ul-Elements hinzu
    ulNode.AppendChild(liNode);
}

// Suche das div-Element mit der Klasse "x-axis"
HtmlNode divNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='x-axis']");

if (divNode != null)
{
    // Erstelle ein neues span-Element mit dem Wert "10"
    HtmlNode spanNode = HtmlNode.CreateNode("<span>"+ strommittel[daten,2] +"</span>");

    // Füge das span-Element als letztes Kind des div-Elements hinzu
    divNode.AppendChild(spanNode);
}
// Speichere die HTML-Datei
htmlDoc.Save("index.html");
        }
        }
        }

