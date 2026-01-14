import "./App.css";
import PolyCanvas from "./PolyCanvas.jsx";
import Toolbar from "./Toolbar.jsx";
import { useState } from "react";

function App() {
  const [points, setPoints] = useState([]);
  const [polygons, setPolygons] = useState([]);
  const [redoPoints, setRedoPoints] = useState([]);
  const [redoPolygons, setRedoPolygons] = useState([]);


  const resetRedoStack = () => {
    setRedoPoints([]);
    setRedoPolygons([]);
  }
  const revertLastDraw = () => {
    if (points.length > 0) {
      setRedoPoints([points.at(-1), ...redoPoints]);
      setPoints(points.slice(0, -1));
    } else if (polygons.length > 0) {
      const newPolygons = [...polygons];
      const lastPolygon = newPolygons.pop();

      setPolygons(newPolygons);
      setPoints(lastPolygon);

      setRedoPolygons([redoPoints, ...redoPolygons]);
      setRedoPoints([]);
    }
  };

  const redoLastDraw = () => {
    if (redoPoints.length > 0) {
      setPoints([...points, redoPoints[0]]);
      setRedoPoints(redoPoints.slice(1));
    } else if (redoPolygons.length > 0) {
      setPolygons([...polygons, points]);

      const nextRedoPoints = redoPolygons[0];
      setRedoPoints(nextRedoPoints);

      setRedoPolygons(redoPolygons.slice(1));
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
