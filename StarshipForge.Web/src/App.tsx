import { useEffect, useRef, useState } from 'react';
import * as THREE from 'three';
import { HubConnectionBuilder, HttpTransportType } from '@microsoft/signalr';
import './App.css';

function App() {
  const mountRef = useRef<HTMLDivElement>(null);
  const sceneRef = useRef<THREE.Scene>();
  const rendererRef = useRef<THREE.WebGLRenderer>();
  const connectionRef = useRef<any>();
  const starshipGroupRef = useRef<THREE.Group>(new THREE.Group());
  const [signalRStatus, setSignalRStatus] = useState('Connecting...');
  const [design, setDesign] = useState<any>(null);
  const [error, setError] = useState<string>('');

  useEffect(() => {
    if (!mountRef.current) return;

    // Initialize Three.js scene
    const scene = new THREE.Scene();
    scene.background = new THREE.Color(0x0a0a0a);
    sceneRef.current = scene;

    const containerWidth = mountRef.current.clientWidth;
    const containerHeight = mountRef.current.clientHeight;

    const camera = new THREE.PerspectiveCamera(75, containerWidth / containerHeight, 0.1, 1000);
    camera.position.z = 15;

    const renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setSize(containerWidth, containerHeight);
    renderer.shadowMap.enabled = true;
    renderer.shadowMap.type = THREE.PCFSoftShadowMap;
    rendererRef.current = renderer;

    mountRef.current.appendChild(renderer.domElement);

    // Add ambient light
    const ambientLight = new THREE.AmbientLight(0x404040, 0.4);
    scene.add(ambientLight);

    // Add directional light
    const directionalLight = new THREE.DirectionalLight(0xffffff, 0.8);
    directionalLight.position.set(10, 10, 5);
    directionalLight.castShadow = true;
    scene.add(directionalLight);

    // Create starship group
    starshipGroupRef.current = new THREE.Group();
    scene.add(starshipGroupRef.current);

    // Add initial placeholder starship
    const geometry = new THREE.CylinderGeometry(1, 1, 5, 32);
    const material = new THREE.MeshLambertMaterial({ color: 0x888888 });
    const starship = new THREE.Mesh(geometry, material);
    starship.castShadow = true;
    starship.receiveShadow = true;
    starshipGroupRef.current.add(starship);

    // Animation loop
    const animate = () => {
      requestAnimationFrame(animate);
      starshipGroupRef.current.rotation.y += 0.01;
      renderer.render(scene, camera);
    };
    animate();

    // Initialize SignalR connection
    const connection = new HubConnectionBuilder()
      .withUrl('http://localhost:5147/designHub', {
        transport: HttpTransportType.WebSockets,
        skipNegotiation: true
      })
      .withAutomaticReconnect()
      .build();

    connection.on('ReceiveStateUpdate', (state) => {
      console.log('Received state update:', state);
      if (state.type === 'design_update' && state.design) {
        setDesign(state.design);
        setError('');
        // Update 3D visualization
        updateStarshipVisualization(state.design);
      } else if (state.type === 'error') {
        setError(state.message);
        console.error('Design error:', state.message);
      }
    });

    const updateStarshipVisualization = (design: any) => {
      // Clear previous geometry
      starshipGroupRef.current.clear();

      if (design.components) {
        let yOffset = 0;

        // Booster (at bottom)
        if (design.components.booster) {
          const boosterGeometry = new THREE.CylinderGeometry(2, 2, 3, 32);
          const boosterMaterial = new THREE.MeshLambertMaterial({ color: 0xff6b35 });
          const booster = new THREE.Mesh(boosterGeometry, boosterMaterial);
          booster.position.y = yOffset;
          booster.castShadow = true;
          starshipGroupRef.current.add(booster);
          yOffset += 3;
        }

        // Starship tanks (middle)
        if (design.components.starship) {
          for (let i = 0; i < design.components.starship.sections; i++) {
            const tankGeometry = new THREE.CylinderGeometry(1.5, 1.5, 2.5, 32);
            const tankMaterial = new THREE.MeshLambertMaterial({ color: 0x004e89 });
            const tank = new THREE.Mesh(tankGeometry, tankMaterial);
            tank.position.y = yOffset;
            tank.castShadow = true;
            starshipGroupRef.current.add(tank);
            yOffset += 2.5;
          }
        }

        // Engines (at top)
        if (design.components.engines) {
          const engineCount = Math.min(design.components.engines.count, 6); // Show max 6 for visibility
          for (let i = 0; i < engineCount; i++) {
            const engineGeometry = new THREE.CylinderGeometry(0.3, 0.3, 0.8, 16);
            const engineMaterial = new THREE.MeshLambertMaterial({ color: 0xffd60a });
            const engine = new THREE.Mesh(engineGeometry, engineMaterial);
            const angle = (i / engineCount) * Math.PI * 2;
            engine.position.x = Math.cos(angle) * 1.5;
            engine.position.z = Math.sin(angle) * 1.5;
            engine.position.y = yOffset + 0.5;
            engine.castShadow = true;
            starshipGroupRef.current.add(engine);
          }
        }
      }
    };

    connection.start()
      .then(() => {
        console.log('Connected to design hub');
        setSignalRStatus('Connected');
      })
      .catch(err => {
        console.error('Connection failed:', err);
        setSignalRStatus('Failed');
      });

    connection.onclose(() => {
      setSignalRStatus('Disconnected');
    });

    connection.onreconnecting(() => {
      setSignalRStatus('Reconnecting...');
    });

    connection.onreconnected(() => {
      setSignalRStatus('Connected');
    });

    connectionRef.current = connection;

    // Handle window resize
    const handleResize = () => {
      if (!mountRef.current) return;
      const newWidth = mountRef.current.clientWidth;
      const newHeight = mountRef.current.clientHeight;
      camera.aspect = newWidth / newHeight;
      camera.updateProjectionMatrix();
      renderer.setSize(newWidth, newHeight);
    };
    window.addEventListener('resize', handleResize);

    // Cleanup
    return () => {
      window.removeEventListener('resize', handleResize);
      if (mountRef.current && renderer.domElement) {
        mountRef.current.removeChild(renderer.domElement);
      }
      renderer.dispose();
      connection.stop();
    };
  }, []);

  const handleDesignRequest = async () => {
    try {
      const response = await fetch('http://localhost:5147/api/design', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ request: 'Design a basic starship with methane fuel capacity' })
      });
      if (response.ok) {
        console.log('Design request sent');
      } else {
        const errorText = await response.text();
        console.error('Design request failed:', response.status, errorText);
      }
    } catch (error) {
      console.error('Failed to send design request:', error);
    }
  };

  return (
    <div className="app">
      <div ref={mountRef} className="three-container" />
      <div className="mission-control-ui">
        <h1>Starship Forge - Mission Control</h1>
        <div className="controls">
          <button onClick={handleDesignRequest} className="design-btn">
            Generate Design
          </button>
        </div>
        <div className="data-panel">
          <h2>System Status</h2>
          <div className="status-item">
            <span>AI Agent:</span>
            <span className="status-active">Active</span>
          </div>
          <div className="status-item">
            <span>3D Engine:</span>
            <span className="status-active">Running</span>
          </div>
          <div className="status-item">
            <span>SignalR:</span>
            <span className={signalRStatus === 'Connected' ? 'status-active' : 'status-inactive'}>{signalRStatus}</span>
          </div>
          
          {error && (
            <div className="error-banner">
              <strong>Error:</strong> {error}
            </div>
          )}
          
          {design && design.performance && (
            <>
              <h2 style={{ marginTop: '20px' }}>Design Metrics</h2>
              <div className="status-item">
                <span>Mass:</span>
                <span>{design.performance.total_mass_kg.toLocaleString()} kg</span>
              </div>
              <div className="status-item">
                <span>Thrust/Weight:</span>
                <span>{design.performance.thrust_to_weight.toFixed(3)}</span>
              </div>
              <div className="status-item">
                <span>Engines:</span>
                <span>{design.components.engines.count}</span>
              </div>
              <div className="status-item">
                <span>Tank Sections:</span>
                <span>{design.components.starship.sections}</span>
              </div>
              <div className="status-item">
                <span>Payload Cap:</span>
                <span>{design.performance.estimated_payload_capacity_kg.toLocaleString()} kg</span>
              </div>
            </>
          )}
        </div>
      </div>
    </div>
  );
}

export default App;
