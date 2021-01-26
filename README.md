# CoronaTest
## Statusbericht
### 26.01.2021
* Projektstruktur ist erstellt
* Import Console ist erstellt => 5 Kampagnen und 11 Testcenter
* SMS Validierung ist erstellt => Umsetzung wie im Livecoding
* WebApplikation mit Grundfunktionalität ist erstellt
    * Startseite (Neuanmeldung / Anmeldung)
    * Page Neuanmeldung mit Eingabe Teilnehmerdaten
        * Mit Validierungen u. a. SVNR Validierung   
    * Page SMS Verifizierung (2 Faktor Authentifizierung)
    * Page Teilnehmer mit Testanmeldungen
    * Page Testanmeldung
        * Mit SMS Bestätigung

Offene Punkte:
* Änderungsmöglichkeit der Stammdaten des Teilnehmers
* Änderungsmöglichkeit der Testung (Ort, Termin, Datum, Uhrzeit)
* Sourcecodeoptimierungen
    * Auswertung Token -> Codeverdoppelung
    * Code auslagen / kapseln

## Projektanforderung
### Web-Seite Razor Page: Abgabe 26.01.2021 um 23:59
Umsetzung einer Web Seite mit Razor Page für folgende Anforderungen:
* Personen (Participant) müssen sich registrieren können
* Nach der Registrierung kann sich die Person für eine Testung (Examination) anmelden/einen Termin für einen freien Slot reservieren.
* Reservierungsbestätigung per SMS-Versand:
    * Identifikation-Nummer
    * Ausgewähler Zeitpunkt der Testung
    * Daten zum TestCenter
* Eine Person kann eine angemeldete Testung auch adaptieren. Es kann ein anderes TestCenter ausgewählt werden bzw. ein anderer Termin gewählt werden.
* Eine Person kann auf der Web Seite ihre Stammdaten (Adresse, etc. selbst verwalten)

### WPF Anwendung für Personal: Abgabe 01.02.2021 um 15:59
* Zu testende Personen müssen sich mit ihrer Identifikationnummer aus der Reservierungsbestätigung anmelden
* Zur Verifikation der Identität muss eine SMS an die angemeldete Testperson inkl. Code übermittelt werden. Dieser Code muss von der Test-Person an den Mitarbeiter im Test-Center übergeben werden.
* Das Testergebnis muss in der Applikation persistiert werden können
* Das Ergebnis wird per SMS an die Testperson übermittelt
* In der Anwenung sollen einfache Auswertungen dargestellt werden:
    * Möglichkeit einer Zeitraumfilterung (von - bis)
    * Anzeige aller Reservierungen (wenn schon getestet -> inkl. Testergebnis) im Filter-Zeitraum
    * Aufsummierung aller positiv- bzw. negativer Testresultate im Filer-Zeitraum