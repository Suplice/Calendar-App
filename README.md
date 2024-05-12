
<h1 align="center">
:calendar: Calendar App :calendar:
</h1>

<br>
<h3>Opis:</h3>

#
<p><b></b>Calendar App to aplikacja Webowa stworzona w technologii .NET MVC , wykorzystująca języki takie jak C#, HTML, CSS oraz JavaScript. <br>
Aplikacja posiada połącznie z lokalną bazą danych. (Planowane jest postawienie aplikacja na hostingu zewnętrznym)</b></p>
<br>
<br>
<p align="left">
<img src="https://skillicons.dev/icons?i=git" />
<img src="https://skillicons.dev/icons?i=cs" />
<img src="https://skillicons.dev/icons?i=html" />
<img src="https://skillicons.dev/icons?i=css" />
<img src="https://skillicons.dev/icons?i=javascript" />
</p>

<b>Aplikacja zapewnia dostęp do kalendarza, w którym możemy m.in:</b>
 - Dodawać nowe eventy
 - Modyfikować istniejące eventy
 - Usuwać eventy
<br>

<b>Zapewniona jest również możliwość tworzenia konta, pozwala nam to na zachowanie zmian dokonanych w kalendarzu. <br>
Posiadając konto możemy wykonywać na nim wiele operacji takich jak:</b>
  - Zmiana nazwy użytkownika
  - Zmiana loginu
  - Zmiana adresu e-mail
  - Zmiana hasła
  - Wylogowanie
  - Zamknięcie konta (<b>Podczas korzystanie z tej opcji należy uważać, gdyż akcja jest ona nieodwracalna!!!</b>)
<br>
<br>

# <p align="center"> Funkcje Programu </p>

<b><h3 align="center">:arrow_forward: Rejestracja i Logowanie: :arrow_backward:</h3></b>
<br>
:red_square: Użytkownik po wejściu na stronę może utworzyć konto za pomocą przycisku "Register" znajdującego się w pasku nwigacyjnym.
<br>
<br>
///Wprowadzić zdjęcie tego///
<br>
<br>

:red_square: Po naciśnięciu na przycisk zostajemy przekierowani na stronę z formularzem.
<br>
<br>
///Zdjęcie formularze rejestrowania///
<br>
<br>

:red_square: Aby zarejestrować się należy wprowadzić dane do odpowiednich pól, a następnie kliknąć na przycisk submit.
<br>
<b> <p align = "center">Należy zwrócić uwagę na wymagania wprowadzanych przez nas danych do formularza: </p></b> <br>
	:large_blue_circle: Login: <b>(Pole jest wymagane, Należy wprowadzić od 3 do 15 znaków)</b> <br> <br>
	:large_blue_circle: Name: <b>(Pole jest wymagane, Należy wprowadzić od 3 do 15 znaków)</b> <br> <br>
	:large_blue_circle: Email: <b>(Pole jest wymagane, Wprowadzony typ danych musi być typem e-mail) </b> <br> <br>
	:large_blue_circle: Password: <b>(Pole jest wymagane, Musi zawierać przynajmniej jedną małą literę, jedną dużą, jedną cyfrę, jeden znak specjalny, Minimalna długośc to 5) </b> <br> <br>
	:large_blue_circle: Confirm Password: <b>(Pole musi spełniać wszystkie warunki pola "Password" oraz wprowadzone dane muszą być identeczny względem Pola "Password") </b> <br>

<br>

:red_square: Po poprawnie wykonanej rejestracji zostaniemy przekierowani na stronę logowania, na której będziemy w stanie się zalogować na utworzone konto

<br>
<br>
///Wprowadzić zdjęcie formy logowania się/// 

:red_square: Aby zalogować się należy wprowadzić dane do odpowiednich pól, a następnie kliknąć submit.
<b> <p align = "center">Należy zwrócić uwagę na wymagania wprowadzanych przez nas danych do formularza: </p></b> <br> 
:large_blue_circle: Login: <b>(Pole jest wymagane)</b> <br> <br>
:large_blue_circle: Password: <b>(Pole jest wymagane)</b> <br> <br>

---

<b><h3 align="center">:arrow_forward: Kalendarz Testowy: :arrow_backward:</h3></b>
<br>
:red_square: Użytkownik może skorzystać z kalendarza testowego bez wstępnego utworzenia konta, w celu przetestowania działania programu.
<br>
///Zdjęcie przycisku Test Calendar///

:red_square:  Podczas korzystania z kalendarza testowego możemy:
<br><br>
 :large_blue_circle: Dodawać eventy (Przycisk Add Event) <br><br>
 :large_blue_circle: Przesuwać eventy <br><br>
 :large_blue_circle: Modyfikować eventy <br><br>
 :large_blue_circle: Usuwać eventy<br><br>

///Dodać zdjęcie dodawania eventu///

:red_square: Podczas edycji lub dodawania elementów musimy zwrócić uwagę na wymagania wprowadzanych danych: <br><br>
 :large_blue_circle: Title (Pole obowiązkowe) <br> <br>
 :large_blue_circle: Description (Pole nieobowiązkowe) <br> <br>
 :large_blue_circle: StartDate (Pole obowiązkowe, Typ danych musi być typu 'Data') <br> <br>
 :large_blue_circle: EndDate (Pole obowiązkowe, Typ danych musi być typu 'Data', Data musi być Późniejsza niż "StartDate") <br> <br> 

 ---

<b><h3 align="center">:arrow_forward: Kalendarz Zalogowanego użytkownika: :arrow_backward:</h3></b> <br>

:red_square: Kalendarz zalogowanego użytkownika obowiązują takie same zasady podczas dodawania/edycji eventu jak kalendarz testowy.
<br><br>
:red_square: Jedyną zmianą w kalendarzu jest zapamiętywanie dokonanych zmian w bazie danych. <br>

---

<b><h3 align="center">:arrow_forward: Ustawienia Konta :arrow_backward:</h3></b>
<br>
:red_square: Strona ustawień użytkownika wygląda następująco: <br>
///Przedstawić zdjęcie stronę ustawień///

:red_square: Na stronie ustawień możemy dokonywać zmian danych użytkownika, wylogować się oraz zamknąć konto <br><br>


:red_square: Podczas zmian danych użytkownika należy zwrócić uwagę na wymagania wprowadzanych danych: <br><br>
  :large_blue_circle: Username <b>(Pole obowiązkowe, musi zawierać między 3 a 15 znaków)</b> <br><br>
  :large_blue_circle: E-mail <b>(Pole jest wymagane, Wprowadzony typ danych musi być typem e-mail) </b> <br> <br>
  :large_blue_circle: Name: <b>(Pole jest wymagane, Należy wprowadzić od 3 do 15 znaków)</b> <br> <br>
  :large_blue_circle: Password: <b>(Pole jest wymagane, Musi zawierać przynajmniej jedną małą literę, jedną dużą, jedną cyfrę, jeden znak specjalny, Minimalna długośc to 5) </b> <br> <br>

:red_square: Można również użyć opcji zamknij konto, która powoduje natychmiastowe wylogowanie oraz usunięcie konta <b>(Podczas używania tej funkcji programu należy zwrócić szczególną uwagę, ponieważ jest ona nieodwracalna)</b> <br> <br>


---

<b><h3 align="center">:arrow_forward: Kilka słów końcowych :arrow_backward:</h3></b> <br>

Proszę pamiętać o tym, że program jest w trakcie rozwoju i mogą występować w nim błędy. <br>
Jeśli natrafisz na takowy, proszę o niezwłoczne poinformowanie mnie o nim w jeden z podanych poniżej sposobów. <br> <br>
### 💬 Contact Me
#
<a href="https://www.linkedin.com/in/mateusz-duma-74b0662a2">
		<img src="https://skillicons.dev/icons?i=linkedin" />
</a>
<a href="mailto:mateuszsuplice@gmail.com">
		<img src="https://skillicons.dev/icons?i=gmail" />
</a>



























