# Kohonen Map
Projekt na studia 2011 r. Kompresja obrazów za pomocą map cech Kohonena

![](/media/lena_iteracji1000_uczenie0,1_siatka10.png)

## Opis problemu

Mapy cech Kohonena, zwane także mapami samoorganizującymi się, są typem sieci neuronowej uczącej się w trybie nienadzorowanym (bez nauczyciela) w celu wyprodukowania zwykle dwuwymiarowej reprezentacji danych wejściowych, zwanej dalej mapą. Ich podstawową różnicą w porównaniu do innych sieci neuronowych jest wykorzystywanie funkcji sąsiedztwa w celu zapamiętania cech topologicznych dla danych wejściowych. Dzięki temu mapy Kohonena są użyteczne podczas wizualizacji danych wielowymiarowych. 

Jak większość sieci neuronowych, mapy Kohonena pracują w dwóch trybach: uczenia się i mapowania. Nauka buduje mapę wykorzystując do tego celu wektory wejściowe, a mapowanie z tych danych tworzy mapę wyjściową. Cechą charakterystyczną tego typu sieci neuronowej jest  tylko jednokierunkowy przepływ informacji oraz wykorzystanie tylko dwóch warstw.

Mapę Kohonena w tym przypadku wykorzystaliśmy do kompresji obrazów. Ten typ sieci świetnie się do tego nadaje, gdyż metoda ta polega na podzieleniu obrazu o wymiarach np. 1024 x 1024 pikseli na mniejsze ramki o wymiarach np. 16x16 pikseli.  Każda z tych ramek stanowi pojedynczy wektor wejściowy x, a ilość neuronów w sieci jest równa ilości wektorów wejściowych. Następnie następuje nauka sieci tak, by każdemu wektorowi x odpowiadał wektor wag neurony zwycięzcy (w naszym wypadku wykorzystaliśmy właśnie ten algorytm). Na końcu zwycięskie neurony tworzą obraz wyjściowy.

## Opis algorytmu

Algorytm w skrócie:
1. Losowanie wag neuronów naszej mapy.
1. Pobieramy wektor wejściowy.
1. Przechodzimy przez każdy węzeł w mapie:
   1. Korzystamy z odległości euklidesowej by znaleźć podobieństwo pomiędzy wektorem wejściowym a wektorem wag
   1. Wyszukujemy neuron o najmniejszej odległości (BMU).
1. Aktualizujemy neurony sąsiedztwa BMU dając je bliżej wektora wejściowego według wzoru:
   * Ww(t + 1) = Ww(t) + Θ(t)α(t)(D(t) - Ww(t)), gdzie:
      * Ww – wektor wag
      * t- iteracja
      * D – wektor wyjściowy
      * Θ(t) – odległość od BMU
      * α(t) – współczynnik nauki
1. Iterujemy aż skończą nam się wektory wejściowe. 

## Wnioski

Kompresja obrazów za pomocą map Kohonena nie jest bezstratna – każdy z obrazów stracił wyraźnie na jakości, choć nie posiadają one żadnych artefaktów, jak to często się zdarza przy wykorzystaniu algorytmu JPG. W naszym wypadku po użyciu tego typu sieci neuronowej otrzymujemy obrazy przypominające pliki gif, Poziom kompresji jest natomiast bardzo wysoki i oscyluje w granicach 7-12. Niestety, czas potrzebny na wykonanie tych operacji jest bardzo długi – 9-10 sekund na kompresję niewielkiego obrazka i nawet do 15-20 minut na duży to stanowczo za długo, by w ten sposób kompresować pliki. Jakość uzyskanych plików w dużej mierze zależy od ich szczegółowości – prostsze obiekty będą wyglądać po kompresji o wiele lepiej niż te złożone.

Największą zaletą tego typu sieci neuronowej jest jego prostota i możliwość nauki bez nadzoru, a także bardzo łatwa interpretacja wyników. Na pierwszy rzut oka wiemy, czy nasz algorytm został przez nas ulepszony (np. poprzez wykorzystanie innej, lepszej funkcji sąsiedztwa). W publikacjach naukowych bardzo często też można spotkać wykorzystanie map Kohonena do prezentacji innych danych w postaci grafu – świadczy to o uniwersalności tego algorytmu.
