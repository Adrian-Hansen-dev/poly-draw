**Funktionale Prinzipien die wir genutzt haben**

- Setzen Immutability mit dem **Spread Operator** (`...`) → kopieren von Arrays anstatt array.push (verändern)
- Higher-Order Functions
    - Funktion die Funktionen als parameter annehmen
- Declarative Style → gewünschte **Endergebnis** anstatt die exakten **Rechenschritte**
    - z.B map() function zum rendern der Polygons

**Was war schwierig?** 

- Unser State Management approach ist mir beim implementieren der Redo-Funktion auf die Füße gefallen
- 2 useStates[]  für Points und Polygons noch verständlich
- 4 useStates[]  f für Points, Polygons, RedoPoints und RedoPolygons nicht mehr
    - Debugging dann sehr schwierig → viel hin und her geschoben wird

**Was kann man noch verbessern?** 

- **"Fragmented” state**
    - 4 Arrays führt schnell zu Fehlern
    - Alternativ: "History"-Array und ein Index anstatt 4 States nur zwei:
    1. **`history`**: Ein Array, das den kompletten Zustand (Punkte + Polygone) zu jedem Zeitpunkt speichert.
    2. **`index`**: Ein Zeiger, der angibt, an welchem Punkt der History wir uns gerade befinden.
- **Strict typing:** JavaScript → TypeScript
- **Single Responsibility Pattern**
    - undo, redo functions in App.jsx → Für die Demo irrelevant
- **Erweiterbarkeit** der Lines und Polygons
    - Components für die Lines und Polygons erstellen, damit mehr Attribute möglich sind: Thickness, Color etc.
