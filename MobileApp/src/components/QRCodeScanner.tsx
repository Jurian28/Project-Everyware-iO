import { useEffect, useRef, useState } from 'react';
import { View, Text, TouchableOpacity } from 'react-native';
import { Html5Qrcode, Html5QrcodeSupportedFormats } from 'html5-qrcode';
import { qrCodeStyles } from '../styles/attendanceStyle';

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
          { fps: 10, qrbox: { width: 260, height: 260 } },
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
        { fps: 10, qrbox: { width: 260, height: 260 } },
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
      <View style={qrCodeStyles.centered}>
        <Text style={qrCodeStyles.errorIcon}>⚠️</Text>
        <Text style={qrCodeStyles.errorText}>{error}</Text>
        <TouchableOpacity style={qrCodeStyles.button} onPress={handleReset}>
          <Text style={qrCodeStyles.buttonText}>Try Again</Text>
        </TouchableOpacity>
        {onClose && (
          <TouchableOpacity style={qrCodeStyles.closeButton} onPress={onClose}>
            <Text style={qrCodeStyles.closeText}>Close</Text>
          </TouchableOpacity>
        )}
      </View>
    );
  }

  return (
    <View>
      <View style={qrCodeStyles.container}>
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

      <View pointerEvents="box-none">
        <View style={qrCodeStyles.bottomOverlay}>
          {starting && <Text style={qrCodeStyles.hint}>Starting camera…</Text>}
          {!starting && !scanned && (
            <Text style={qrCodeStyles.hint}>Align QR code within the frame</Text>
          )}
          {scanned && <Text style={qrCodeStyles.hint}>QR code scanned!</Text>}
          {scanned && (
            <TouchableOpacity style={qrCodeStyles.button} onPress={handleReset}>
              <Text style={qrCodeStyles.buttonText}>Scan Again</Text>
            </TouchableOpacity>
          )}
          {onClose && (
            <TouchableOpacity style={qrCodeStyles.closeButton} onPress={onClose}>
              <Text style={qrCodeStyles.closeText}>Close</Text>
            </TouchableOpacity>
          )}
        </View>
      </View>
    </View>
  );
}