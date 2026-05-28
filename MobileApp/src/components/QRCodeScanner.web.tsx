import { useEffect, useRef, useState } from 'react';
import { View, Text, TouchableOpacity } from 'react-native';
import { Html5Qrcode, Html5QrcodeSupportedFormats } from 'html5-qrcode';
import { webStyles } from '../styles/qrScannerStyle';

type QRScannerProps = {
  onScan: (data: string) => void;
  onClose?: () => void;
};

const SCANNER_ID = 'qr-scanner-web';

export default function QRScanner({ onScan, onClose }: QRScannerProps) {
  const scannerRef = useRef<Html5Qrcode | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [scanned, setScanned] = useState(false);
  const [starting, setStarting] = useState(true);

  useEffect(() => {
    let scanner: Html5Qrcode | null = null;

    if (!document.getElementById('qr-scanner-styles')) {
      const style = document.createElement('style');
      style.id = 'qr-scanner-styles';
      style.textContent = `
        #${SCANNER_ID} {
          width: 100% !important;
        }
        #${SCANNER_ID} video {
          height: 100% !important;
          object-fit: cover !important;
        }
      `;
      document.head.appendChild(style);
    }

    const start = async () => {
      try {
        scanner = new Html5Qrcode(SCANNER_ID, {
          formatsToSupport: [Html5QrcodeSupportedFormats.QR_CODE],
          verbose: false,
        });
        scannerRef.current = scanner;

        await scanner.start(
          { facingMode: 'environment' },
          { fps: 20, qrbox: { width: 280, height: 280 } },
          (decodedText) => {
            setScanned(true);
            stopScanner(scanner!);
            onScan(decodedText);
          },
        );

        setStarting(false);
      } catch (err: any) {
        setError(
          err?.message?.includes('Permission')
            ? 'Camera permission denied. Please allow camera access and reload.'
            : 'Could not start camera. Make sure no other app is using it.'
        );
        setStarting(false);
      }
    };

    start();

    return () => {
      stopScanner(scannerRef.current);
    };
  }, []);

  const stopScanner = async (scanner: Html5Qrcode | null) => {
    if (!scanner) return;

    try {
      if (scanner.isScanning) {
        await scanner.stop();
      }

      await scanner.clear();
    } catch (e) {
      console.error(e);
    }
  };

  const handleReset = async () => {
    setScanned(false);
    setError(null);
    setStarting(true);

    const scanner = new Html5Qrcode(SCANNER_ID, {
      formatsToSupport: [Html5QrcodeSupportedFormats.QR_CODE],
      verbose: false,
    });
    scannerRef.current = scanner;

    try {
      await scanner.start(
        { facingMode: 'environment' },
        { fps: 20, qrbox: { width: 280, height: 280 } },
        (decodedText) => {
          setScanned(true);
          stopScanner(scanner);
          onScan(decodedText);
        },
        () => {}
      );
      setStarting(false);
    } catch {
      setError('Could not restart camera.');
      setStarting(false);
    }
  };

  if (error) {
    return (
      <View style={webStyles.centered}>
        <Text style={webStyles.errorIcon}>⚠️</Text>
        <Text style={webStyles.errorText}>{error}</Text>
        <TouchableOpacity style={webStyles.button} onPress={handleReset}>
          <Text style={webStyles.buttonText}>Try Again</Text>
        </TouchableOpacity>
        {onClose && (
          <TouchableOpacity style={webStyles.closeButton} onPress={onClose}>
            <Text style={webStyles.closeText}>Close</Text>
          </TouchableOpacity>
        )}
      </View>
    );
  }

  return (
    <View>
      <View style={webStyles.container}>
        {/* This needs to be a div */}
        <div
          id={SCANNER_ID}
          style={{
            width: 300,
            height: 375,
            position: 'absolute' as any,
            top: '50%',
            left: '50%',
            transform: 'translate(-50%, -50%)',
            overflow: 'hidden' as any,
            backgroundColor: '#000',
          }}
        />
      </View>

      <Overlay
        scanned={scanned}
        starting={starting}
        handleReset={handleReset}
        onClose={onClose}
      />
    </View>
  );
}

function Overlay({ scanned, starting, handleReset, onClose }: { scanned: boolean; starting: boolean; handleReset: () => void; onClose?: () => void }) {
  return (
    <View pointerEvents="box-none">
      <View style={webStyles.bottomOverlay}>
        {starting && <Text style={webStyles.hint}>Starting camera…</Text>}
        {!starting && !scanned && (
          <Text style={webStyles.hint}>Align QR code within the frame</Text>
        )}
        {scanned && <Text style={webStyles.hint}>QR code scanned!</Text>}
        {scanned && (
          <TouchableOpacity style={webStyles.button} onPress={handleReset}>
            <Text style={webStyles.buttonText}>Scan Again</Text>
          </TouchableOpacity>
        )}
        {onClose && (
          <TouchableOpacity style={webStyles.closeButton} onPress={onClose}>
            <Text style={webStyles.closeText}>Close</Text>
          </TouchableOpacity>
        )}
      </View>
    </View>
  )
}