import { useEffect, useRef, useState } from 'react';
import { StyleSheet, View, Text, TouchableOpacity } from 'react-native';
import { Html5Qrcode, Html5QrcodeSupportedFormats } from 'html5-qrcode';
import colors from '../../enums/colors';

type QRScannerProps = {
  onScan: (data: string) => void;
  onClose?: () => void;
};

const SCANNER_ID = 'qr-scanner-web';

function QRScanner({ onScan, onClose }: QRScannerProps) {
  const scannerRef = useRef<Html5Qrcode | null>(null);
  const scannedRef = useRef(false);
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
            console.log('✅ QR Code Detected:', decodedText);
            if (scannedRef.current) return;

            scannedRef.current = true;
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
      <View style={styles.centered}>
        <Text style={styles.errorIcon}>⚠️</Text>
        <Text style={styles.errorText}>{error}</Text>
        <TouchableOpacity style={styles.button} onPress={handleReset}>
          <Text style={styles.buttonText}>Try Again</Text>
        </TouchableOpacity>
        {onClose && (
          <TouchableOpacity style={styles.closeButton} onPress={onClose}>
            <Text style={styles.closeText}>Close</Text>
          </TouchableOpacity>
        )}
      </View>
    );
  }

  return (
    <View>
      <View style={styles.container}>
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

      {/* <View style={styles.overlay} pointerEvents="box-none"> */}
      <View pointerEvents="box-none">
        <View style={styles.bottomOverlay}>
          {starting && <Text style={styles.hint}>Starting camera…</Text>}
          {!starting && !scanned && (
            <Text style={styles.hint}>Align QR code within the frame</Text>
          )}
          {scanned && <Text style={styles.hint}>QR code scanned!</Text>}
          {scanned && (
            <TouchableOpacity style={styles.button} onPress={handleReset}>
              <Text style={styles.buttonText}>Scan Again</Text>
            </TouchableOpacity>
          )}
          {onClose && (
            <TouchableOpacity style={styles.closeButton} onPress={onClose}>
              <Text style={styles.closeText}>Close</Text>
            </TouchableOpacity>
          )}
        </View>
      </View>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    position: 'relative' as any,
    overflow: 'hidden' as any,
  },
  centered: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: '#0A0A0A',
    padding: 24,
    gap: 12,
  },
  errorIcon: {
    fontSize: 40,
    marginBottom: 8,
  },
  errorText: {
    fontSize: 15,
    textAlign: 'center',
    opacity: 0.8,
    marginBottom: 8,
  },
  bottomOverlay: {
    flex: 1,
    alignItems: 'center',
    paddingTop: 28,
    gap: 12,
  },
  hint: {
    fontSize: 15,
    opacity: 0.85,
    letterSpacing: 0.3,
  },
  button: {
    backgroundColor: colors.DEFAULT_BUTTON_COLOR,
    paddingHorizontal: 28,
    paddingVertical: 12,
    borderRadius: 8,
    marginTop: 4,
  },
  buttonText: {
    color: '#FFF',
    fontWeight: '700',
    fontSize: 15,
  },
  closeButton: {
    paddingHorizontal: 20,
    paddingVertical: 10,
  },
  closeText: {
    color: 'rgba(255,255,255,0.5)',
    fontSize: 14,
  },
});

export default function EventsAttendance() {
  return (
    <View style={styles.container}>
      <View style={{ width: '100%', height: '25%' }}>
        <Text style={{ textAlign: 'center', marginTop: 16 }}>
          Web QR Scanner
        </Text>
      </View>
      <QRScanner
        onScan={(data) => {
          console.log(`Scanned QR Code: ${data}`);
        }}
      />
    </View>
  )
};