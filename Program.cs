
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
   string s =reader2.ReadLine();
   string[] values = s.split(';');
   string zeitpunkt = values[0];
   string stromkwh = values[4];
 }
/* -------------------------------------------------- 
   Stage 4: Ausgaben / Visualisierung erzeugen und speichern 
-----------------------------------------------------*/

    
        }
        }
}