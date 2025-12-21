import { Canvas } from "@react-three/fiber";
import { Line, OrthographicCamera } from "@react-three/drei";

export function PolyCanvas() {
  const points = [
    [0, 2],
    [0, 1],
    [2, 2],
    [0, 2],
  ];
  return (
    <div style={{ width: "100vw", height: "100vh" }}>
      <Canvas>
        <OrthographicCamera makeDefault position={[0, 0, 10]} zoom={100} />
        <Line points={points} color="black" lineWidth={1}></Line>
      </Canvas>
    </div>
  );
}
