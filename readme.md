# Dokumentace Rastrový Grafický Editor
Projekt vytvořený v C# a WPF.

---

## Funkce
- Režimy kreslení (tvary, štětec)
- Ukliádání a načítání souborů ve formátu PNG
- Nastavení barvy okraje a výplně
- Průhledný režim pro výplň
- Možnost nastavení tloušťky štětce
- Výběr tvarů (čtverec, kruh, trojúhelník, obdélník)
- Mazání obsahu plátna
---

## Struktura projektu
### Soubory
- `Settings.cs`: Obsahuje globální nastavení aplikace, jako jsou barvy, tloušťka a režim kreslení.
- `MainWindow.xaml.cs`: Hlavní logika aplikace, zpracování událostí uživatelského rozhraní.
- `DrawShape.cs`: Vytvoří a vykresluje tvary na plátno.
- `Files.cs`: Hlavní logika ukládání, načítání a práce se soubory.

---

## `MainWindow.xaml.cs`
### Inicializace
- Nastavuje výchozí hodnoty pro tloušťku štětce, barvu okraje a barvu výplně.

### Události
- **Posuvník velikosti štětce**: Aktualizuje tloušťku štětce.
- **Výběr tvarů**: Mění režim kreslení na základě vybraného tvaru.
- **Výběr barev**: Umožňuje uživatelům vybrat barvy okraje a výplně.
- **Průhledný režim**: Umožňujě nechat barvu výplně prázdnout.

### Správa souborů
- Stará se o funkčnost tlačítek na manipulaci se souborem
- **Uložit**: Uloží plátno jako soubor PNG.
- **Načíst**: Načte soubor PNG na plátno.
- **Vymazat**: Vymaže veškerý obsah z plátna.

---

## Ostatní
### `Settings`
Statická třída, která ukládá globální nastavení:
- `borderColor`: Barva okraje tvaru.
- `fillColor`: Barva výplně tvarů.
- `thickness`: Tloušťka štětce.
- `drawingMode`: Aktuální režim kreslení (`None` nebo `Shape`).
- `currentShape`: Aktuálně kreslený tvar.

---

### `DrawingMode`
Režimy kreslení:
- `None`: Kreslení myší.
- `Shape`: Kreslení tvarů.

---

### `Files`
Stará se o ukládání a načítání souborů. 
- Vlastnost **Saved**, která induje zda aktuální stav plátna byl uložen pro případ, že by uživatel chtěl načíst soubor bez uložení toho co už udělal.
- Metody pro ukládání a načítání souborů:
  - `Save`: Uloží plátno jako PNG.
  - `Load`: Načte PNG soubor do plátna.
  - `Clear`: Vymaže obsah plátna.

---

### `DrawShape`
Třída pro kreslení tvarů na plátno.
Vytvořením objektu ze třídy se vytvoří tvar, pomocí počáteční pozice a typu tvaru, který se následně vykreslí na plátno pomocí metody `Draw`. Konečná pozice může být měněna a pokaždé znovu zavolána metoda `Draw`, aby se tvar překreslil na nové souřadnice.

---

### `ShapeType`
Tvary, které mohou být kresleny:
- `Square`
- `Circle`
- `Triangle`
- `Rectangle`