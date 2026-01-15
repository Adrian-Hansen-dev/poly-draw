import "./App.css";
import PolyCanvas from "./PolyCanvas.jsx";
import Toolbar from "./Toolbar.jsx";
import { useState } from "react";

function App() {
  const [points, setPoints] = useState([]);
  const [polygons, setPolygons] = useState([]);
  const [redoPoints, setRedoPoints] = useState([]);
  const [redoPolygons, setRedoPolygons] = useState([]);

  // Leert den Redo-Stack, wenn ein neuer Punkt hinzugefügt wird
  const resetRedoStack = () => {
    setRedoPoints([]);
    setRedoPolygons([]);
  }

  const revertLastDraw = () => {
    // Es gibt noch Punkte im aktuellen Polygon
    if (points.length > 0) {
      //Punkt entfernen und in den Redo-Array verschieben
      setRedoPoints([points.at(-1), ...redoPoints]);
      setPoints(points.slice(0, -1));
    }
    // Es gibt keine Punkte mehr im aktuellen Polygon, aber es gibt vorherige Polygone
    else if (polygons.length > 0) {
      // vorheriges Polygon entfernen und in den Point-Array verschieben...
      const lastPolygon = polygons.at(-1);
      setPolygons(polygons.slice(0, -1));
      setPoints(lastPolygon);

      // und entferntes Polygon in den Redo-Array verschieben
      setRedoPolygons([redoPoints, ...redoPolygons]);
      setRedoPoints([]);
    }
  };

  const redoLastDraw = () => {
    // Es gibt Punkte im Redo-Array
    if (redoPoints.length > 0) {
      // Punkt aus dem Redo-Array wiederherstellen
      setPoints([...points, redoPoints[0]]);
      setRedoPoints(redoPoints.slice(1));

    }
    // Es gibt keine Punkte mehr im Redo-Array, also das Polygon in den Polygon-Array verschieben
    else if (redoPolygons.length > 0) {
      // Die wiederhergestellten Punkte in den Polygon-Array verschieben
      setPolygons([...polygons, points]);

      // und das nächste Redo-Polygon in den Redo-Points-Array verschieben
      setRedoPoints(redoPolygons[0]);
      setRedoPolygons(redoPolygons.slice(1));

      // den aktuellen Points-Array leeren, weil das Polygon abgeschlossen
      setPoints([]);
    }
  };
  return (
    <div className="app">
      <Toolbar onUndo={revertLastDraw} onRedo={redoLastDraw}></Toolbar>
      <PolyCanvas
        points={points}
        setPoints={setPoints}
        resetRedoStack={resetRedoStack}
        polygons={polygons}
        setPolygons={setPolygons}
      />
    </div>
  );
}

export default App;
