/*************************************
Fertigstellung: 08.01.2022
Aufgabenvariante: A-Klimadaten
Bearbeiter(in) 1: Sahler, Kerstin, 979366
Bearbeiter(in) 2:
***************************************/
/*
Gelbgruene Warnungen zur Laufzeit: Es wurden mehrere Warnhinweise angezeigt.
1) moegliche NULL-Werte
2) verschiedene GDI+-Methoden werden verwendet, die nur fuer Windows verfuegbar
sind.
Bereinigungsversuche:
1) try/catch Block um moegliche NULL-Werte zu ueberspringen und gar nicht erst
mit in die Datei mit den extrahierten Werten zu uebernehmen.
2) es ist notwendig, wie in der Vorlesung besprochen, fuer MAC-Geraete die
zurzeit noch verfuegbaren GDI+-packages zu inkludieren.
Die Fehlermeldungen werden trotzdem weiterhin mit angezeigt.
*/
using System;
using System.Net;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Globalization;
namespace bwi40321 {
class Program {
static void Main(string[] args) {
//Kopfzeile fuer Ausgabe auf der Konsole
Console.WriteLine("***** A-Klimadaten *****\n");
/*****************************************************************
Aktuelle Daten aus einer Datenquelle aus dem Internet laden.
*****************************************************************/
//Es wird ein WebClient-Objekt fuer den Internetzugriff erzeugt
WebClient client = new WebClient();
//Die URL des Webservice wird als Variable gespeichert
string url = "https://cbrell.de/bwi403/demo/getKlimaWS.php";
/*****************************************************************
Die geladenen Daten abspeichern.
*****************************************************************/
//Konsolenausgabe, um den Nutzer ueber den aktuellen Status zu
informieren
Console.WriteLine("Die Daten werden vom Webservice geladen...\n");
//Die URL wird geoeffnet und mit dem Reader werden die Daten aus dem
Webservice eingelesen
Stream data = client.OpenRead(url);
StreamReader reader = new StreamReader(data);
//Die eingelesenen Daten aus dem Webservice werden in einer String-
Variable gespeichert
string sData = reader.ReadToEnd();
//Alle Verbindungen werden geschlossen
data.Close();
reader.Close();
//Konsolenausgabe, um den Nutzer ueber den aktuellen Status zu
informieren
Console.WriteLine("Die Daten aus dem Webservice werden in der Datei
ein-Sahler.csv gespeichert...");
//Das in der String-Variable Gespeicherte wird in eine neue Datei
geschrieben
File.WriteAllText("ein-Sahler.csv", sData);
//Es wird geprueft, ob die Datei vorhanden ist und der Nutzer
informiert
if(File.Exists("ein-Sahler.csv")) {
Console.WriteLine("Die Datei ein-Sahler.csv wurde erstellt.\n");
}
else {
Console.WriteLine("Die Datei ein-Sahler.csv ist nicht
vorhanden.\n");
}
/*****************************************************************
Aus den Daten Werte extrahieren
Daten: Temperatur
*****************************************************************/
//Ein Dateistrom wird erzeugt, um aus der zuvor erstellten Datei
einlesen zu koennen
StreamReader reader2 = new StreamReader("ein-Sahler.csv");
//Eine neue Datei wird erstellt, in die die zu extrahierenden Daten
geschrieben werden koennen
StreamWriter writer = new StreamWriter("aus-Sahler.csv");
//Es werden zwei Listen deklariert, um in ihnen die Werte fuer Zeit und
Temperatur seperat zwischenzuspeichern
List<string> time = new List<string> ();
List<string> temperatures = new List<string> ();
//Da die erste Zeile der Datei als erstes gelesen wird, ist isFirstLine
zu Beginn true
Boolean isFirstLine = true;
//Konsolenausgabe, um den Nutzer ueber den aktuellen Status zu
informieren
Console.WriteLine("Die Daten aus ein-Sahler.csv werden extrahiert und
in aus-Sahler.csv gespeichert...");
//Mithilfe einer Schleifen wird die ein-Datei zeilenweise
durchgegangen, solange das Ende der Datei nicht erreicht ist
while(!reader2.EndOfStream) {
//Moegliche Ausnahmen werden abgefangen (Null-Werte)
try {
//Die Datei wird zeilenweise gelesen und in einer String-
Variable gespeichert
string s = reader2.ReadLine();
//Dieser String wird an den ";" getrennt und in einem String-
Array gespeichert um so die einzelnen Spalten voneinenander zu trennen
string[] extractedValues = s.Split(';');
//Die Werte fuer die Nummer, Zeit und Temperatur werden in
verschiedenen String-Variablen gespeichert
//extractedValues[0] entspricht der 1. Spalte der csv-Datei
string nr = extractedValues[0];
//extractedValues[2] entspricht der 3. Spalte der csv-Datei
string timestamp = extractedValues[2];
//extractedValues[3] entspricht der 4. Spalte der csv-Datei
string temperature = extractedValues[3];
//Anschliessend wird ein neuer String zusammengestellt der die
einzelnen Variablen miteinander verkettet und mit einem ";" trennt
string extractedData = nr + ";" + timestamp + ";" +
temperature;
//Wenn isFirstLine nicht true ist, befindet man sich nicht in
der ersten Zeile und die Anweisung soll ausgefuehrt werden
if (isFirstLine != true) {
//Der Zeitstempel und die Temperatur werden in der
jeweiligen Liste zwischengespeichert
time.Add(timestamp);
temperatures.Add(temperature);
}
//Wenn isFirstLine true ist, ist die erste Zeile durchlaufen
und kann auf false gesetzt werden, um so die erste Zeile zu ueberspringen
else {
isFirstLine = false;
}
/*****************************************************************
Die Ergebnis-Werte abspeichern
*****************************************************************/
//Der String mit den verketteten Werten wird zeilenweise in die
aus-Sahler.csv Datei geschrieben
writer.WriteLine(extractedData);
}
//Falls ein Null-Wert bzw. leere Zeile vorkommt, soll in der
Schleife fortgefahren werden.
catch (Exception e) {
continue;
}
}
//Es wird geprueft, ob die Datei vorhanden ist und der Nutzer
informiert
if(File.Exists("aus-Sahler.csv")) {
Console.WriteLine("Die Datei aus-Sahler.csv wurde erstellt.");
//Konsolenausgabe, um den Nutzer ueber den aktuellen Status zu
informieren
Console.WriteLine("Es wurden "+ time.Count + " Datenpaare
gespeichert.\n");
}
else {
Console.WriteLine("Die Datei aus-Sahler.csv ist nicht vorhanden.");
}
//Alle Verbindungen werden geschlossen
reader2.Close();
writer.Close();
/*****************************************************************
Die Ergebnis-Werte ggf. zu Kennzahlen verdichten
*****************************************************************/
//Deklarieren der Variablen fuer die x- und y-Koordinate
int coordinateX;
int coordinateY;
//Der counter zaehlt die Durchlaeufe und hilft so die Abstaende
zwischen den Koordinaten zu berechnen
int counter = 0;
//gesamte Hoehe/Breite der Bitmap
int total = 768;
//Der linke Rand wird bei 38 pixel festgelegt (an dieser Stelle
verlaeuft spaeter die y-Achse)
int leftBound = 38;
//Der rechte Rand wird bei 722 festgelegt (768 pixel totale Breite - 38
pixel fuer den rechten Rand)
int rightBound = total - leftBound;
//Der obere Rand wird bei 25 pixel festgelegt
int topBound = 25;
//Der untere Rand wird bei 650 pixel festgelegt
int bottomBound = 650;
//Die x-Achse wird bei Pixel 480 gezogen
int xAxis = 480;
//Ein neues PointF Array in Groesse der Liste wird erzeugt, um hier
spaeter die Koordinatenpunkte fuer die Grafik speichern zu koennen
PointF[] coordinates= new PointF[temperatures.Count];
//Die Liste wird von hinten nach vorne durchlaufen, um so von dem
aeltesten Wertepaar bis zum neusten die Koordinaten zu erstellen
for(int i = time.Count - 1; i >= 0; i--) {
//Die x-Koordinate wird berechnet, indem zunaechst der rechte
Bildrand durch die Anzahl der Werte geteilt wird. Der Quotient wird mit
//dem Counter multipliziert und abschliessend die Pixelanzahl des
rechten Bildrandes addiert
coordinateX = (rightBound / time.Count) * counter + leftBound;
//Die Temperatur wird von einer Zeichenkette in eine double
Variable konvertiert. Um Sicherzustellen, dass auch Zahlen ohne
//Nachkommastelle korrekt konvertiert werden, wird das
Trennungszeichen ergaenzt
NumberFormatInfo provider = new NumberFormatInfo();
provider.NumberDecimalSeparator = ".";
double numberY = Convert.ToDouble(temperatures[i], provider);
//Die Temperatur wird in eine Y-Koordinate umgewandelt, indem sie
mit 10 multipliziert und in eine int-Variable konvertiert wird.
//So wird eine bessere Sichtbarkeit von Temperaturschwankungen im
Nachkommabereich gewaehrleistet
coordinateY = xAxis - ((int) numberY * 10);
//Ein temporaeres Objekt vom Typ PointF wird erstellt, in das die
Werte von den zuvor erstellten X- und Y-Koordinaten uebergeben werden
PointF tmp = new PointF(coordinateX, coordinateY);
//In das coordinates-Array wird an der Position des aktuellen
Counters das temporaere Objekt mit den Koordinaten X, Y gespeichert
coordinates[counter] = tmp;
//der Zaehler wird um 1 erhoeht
counter++;
}
/*****************************************************************
Aus den Ergebnis-Werten eine (Informations-)Grafik erstellen.
*****************************************************************/
//Erstellen einer neuen Bitmap in der Groesse 768x768 Pixel
Bitmap bitmap = new Bitmap(total,total);
//Erstellen einer Zeichenoberflaeche auf der Bitmap
Graphics graphic = Graphics.FromImage(bitmap);
//Der schwarze Hintergrund der Zeichenoberflaeche wird mit einem
weissen Rechteck uebermalt
graphic.FillRectangle(new SolidBrush(Color.White), 0, 0, total, total);
//Festlegen des ersten Stifts zum Zeichnen auf der Zeichenoberflaeche
in schwarz mit einer Dicke von 2
Pen p1 = new Pen(Color.Black, 2);
//Festlegen des zweiten Stifts zum Zeichnen auf der Zeichenoberflaeche
in rot mit einer Dicke von 3
Pen p2 = new Pen(Color.IndianRed, 3);
//Koordinaten fuer die Verbindungspunkte der y-Achse (gerade Linie von
Punkt1: x38, y25 bis Punkt2: x38, y650)
int x1 = leftBound;
int y1 = topBound;
int y3 = bottomBound;
//Koordinaten fuer die Verbindungspunkte der x-Achse (gerade Linie von
Punkt1: x38, y380 bis Punkt2: x760, y380)
int x2 = rightBound;
int y2 = xAxis;
//zeichnen der x-Achse in schwarz mit einer Dicke von 2
graphic.DrawLine(p1, x1, y1, x1, y3);
//zeichnen der y-Achse in schwarz mit einer Dicke von 2
graphic.DrawLine(p1, x1, y2, x2, y2);
//Kurve mit dem roten Stift, entlang der Koordinaten, die im PointF
Array coordinates hinterlegt sind, zeichnen
graphic.DrawCurve(p2, coordinates);
//Zeichenketten in Grafik einzeichnen: gefunden ueber die Punkt-
Notation in Visual Studio Code, Dokumentation unter folgender Quelle nachgelesen:
//https://docs.microsoft.com/de-
de/dotnet/api/system.drawing.graphics.drawstring?view=dotnet-plat-ext-6.0
//Festlegen der Schriftfarbe
SolidBrush drawBrush = new SolidBrush(Color.Black);
//Festlegen Schriftart mit verschiedenen Schriftgroessen
Font drawFontMini = new Font("Arial", 7);
Font drawFontSmall = new Font("Arial", 10);
Font drawFontBig = new Font("Arial", 14);
//geschaetzte Koordinaten fuer die Zeichenkette "Name des Studierenden"
Kerstin Sahler
PointF coordinatesName = new PointF(2, 730);
//String mit Namen malen (Schriftgroesse 14, Schriftfarbe schwarz)
graphic.DrawString("Kerstin Sahler", drawFontBig, drawBrush,
coordinatesName);
//geschaetzte Koordinaten fuer die Zeichenkette "Bezeichnung der y-
Achse": Temperature
PointF coordinatesTextYAxis = new PointF(2, 2);
//String mit Bezeichnung der y-Achse malen (Schriftgroesse 10,
Schriftfarbe schwarz)
graphic.DrawString("Temperature", drawFontSmall, drawBrush,
coordinatesTextYAxis);
//geschaetzte Koordinaten fuer die Zeichenkette "Bezeichnung der x-
Achse": Time
PointF coordinatesTextXAxis = new PointF(730, 470);
//String mit Bezeichnung der x-Achse malen (Schriftgroesse 10,
Schriftfarbe schwarz)
graphic.DrawString("Time", drawFontSmall, drawBrush,
coordinatesTextXAxis);
//Schleife, um die y-Achse mit Werten von -10 bis 40 in 10er Schritten
zu beschriften
for (int i = -10; i <= 40; i += 10) {
//Variable y ist die y-Koordinate fuer die Platzierung des
gezeichneten Strings auf der Zeichenoberflaeche, ausgehend
//von 480 als Nullwert des Koordinatensystems. Weitere 10 Pixel
werden wegen der Schriftgroesse abgezogen.
int y = xAxis - (i * 10) - 10;
//Die Koordinaten x = 0 und des ermittelten y werden in ein PointF-
Objekt uebergeben
PointF coordinatePoints = new PointF(0, y);
//Die Gradzahlen werden gezeichnet. Hierfuer wird die Zaehlvariable
i in einen String umgewandelt (Schriftgroesse 10, Schriftfarbe schwarz)
graphic.DrawString(Convert.ToString(i) +"°C", drawFontSmall,
drawBrush, coordinatePoints);
}
//Der zuvor genutzte counter wird auf 0 gesetzt und kommt hier erneut
zur Verwendung.
counter = 0;
//Mithilfe einer Schleife wird die x-Achse mit den Werten des
Zeitstempels (nur Uhrzeit) beschriftet
//Auch hier wird die Liste von hinten nach vorne durchlaufen, um so von
dem aeltesten Wertepaar bis zum neusten die Uhrzeiten zu entnehmen
for (int i = time.Count - 1; i >= 0; i--) {
//Der Listeneintrag an der Stelle i wird am Leerzeichen aufgeteilt
und in einem String-Array gespeichert.
string[] t = time[i].Split(' ');
//Der Wert des Array an der Stelle 0 entspricht dem Datum und wird
in einer String-Variable gespeichert.
string date = t[0];
//Der Wert des Array an der Stelle 1 entspricht der Uhrzeit und
wird in einer weiteren String-Variable gespeichert.
string timeOfDay = t[1];
//Die Koordinaten zum Zeichnen der Uhrzeit werden in einem PointF-
Objekt gespeichert
//Die x-Koordinate wird wieder berechnet, indem zunaechst der
rechte Bildrand durch die Anzahl der Werte geteilt wird. Der Quotient wird mit
//dem Counter multipliziert und abschliessend die Pixelanzahl des
rechten Bildrandes addiert. Weitere 13 Pixel werden wegen der Breite des
//Textes abgezogen (sodass dieser etwa mittig ist).
PointF coordinatesTime = new PointF(((rightBound/time.Count) *
counter + leftBound) - 13, 487);
//Mithilfe einer Verzweigung wird zusaetzlich das Datum mit
eingezeichnet, wenn es 00:00 Uhr ist und der Datumswechsel stattfindet.
if(timeOfDay.Equals("00:00")) {
graphic.DrawString(timeOfDay + "\n" + date, drawFontMini,
drawBrush, coordinatesTime);
}
//Sonst wird nur die Uhrzeit eingezeichnet.
else{
graphic.DrawString(timeOfDay, drawFontMini, drawBrush,
coordinatesTime);
}
//Der Counter wird um 1 erhoeht
counter++;
}
/*****************************************************************
Die Grafik als Bild abspeichern.
*****************************************************************/
//Konsolenausgabe, um den Nutzer ueber den aktuellen Status zu
informieren.
Console.WriteLine("Das Diagramm wird unter info-Sahler.png
gespeichert...");
//Die Bitmap wird als png-Datei gespeichert.
bitmap.Save("info-Sahler.png", ImageFormat.Png);
//Es wird geprueft, ob die Datei vorhanden ist und der Nutzer
informiert
if(File.Exists("info-Sahler.png")) {
Console.WriteLine("Die Datei info-Sahler.png wurde erstellt.\n");
}
else {
Console.WriteLine("Die Datei info-Sahler.png ist nicht
vorhanden.\n");
}
}
}
}
