import { Canvas } from "@react-three/fiber";
import { Line, OrthographicCamera } from "@react-three/drei";
import { useState } from "react";

function PolyCanvas({ points, setPoints, polygons, setPolygons, resetRedoStack }) {
  const [preview, setPreview] = useState([]);

  return (
    <div className="canvas">
      <Canvas>
        <mesh
          position={[0, 0, -1]}
          onPointerMove={(e) => {
            const { x, y } = e.point;
            if (points.length > 0) {
              // Linie zwischen letzem Punkt und Mausposition als Vorschau zeichnen
              setPreview([points[points.length - 1], [x, y]]);
            }
          }}
          onPointerOut={() => {
            setPreview([]);
          }}
          onClick={(e) => {
            const { x, y } = e.point;
            resetRedoStack();
            setPoints((prev) => [...prev, [x, y]]);
          }}
          onDoubleClick={(e) => {
            const { x, y } = e.point;
            setPoints((prev) => [...prev, [x, y]]);
            setPolygons((prev) => [...prev, points]);
            setPoints([]);
            setPreview([]);
          }}
        >
          <planeGeometry args={[100, 100]} />
          <meshBasicMaterial color="#ffffff" />
        </mesh>
        <OrthographicCamera makeDefault position={[0, 0, 10]} zoom={100} />
        {/* draw current points */}
        {points.length > 1 && (
          <Line points={points} color="black" lineWidth={2} />
        )}
        {/* draw preview */}
        {preview.length > 0 && (
          <Line points={preview} color="red" lineWidth={2} />
        )}
        {/* draw polygons */}
        <group>
          {polygons.map((poly, index) => (
            <Line key={index} points={poly} color="blue" lineWidth={2} closed />
          ))}
        </group>
      </Canvas>
    </div>
  );
}

export default PolyCanvas;
