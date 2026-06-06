import { useEffect, useRef, useState } from 'react';
import { StyleSheet, Text, View, TouchableOpacity, Alert, Linking } from 'react-native';
import { useCameraPermission } from 'react-native-vision-camera';
import { CodeScanner } from 'react-native-vision-camera-barcode-scanner';
import { nativeStyles } from '../styles/qrScannerStyle';

type QRScannerProps ={
  onScan: (data: string) => void;
  onClose?: () => void;
}

export default function QRScanner({ onScan, onClose }: QRScannerProps) {
  const { hasPermission, requestPermission } = useCameraPermission();
  const [isActive, setIsActive] = useState(true);
  const scannedRef = useRef(false);

  useEffect(() => {
    if (!hasPermission) {
      requestPermission().then((granted) => {
        if (!granted) {
          Alert.alert(
            'Camera Permission Required',
            'Please enable camera access in your device settings to scan QR codes.',
            [
              { text: 'Cancel', style: 'cancel' },
              { text: 'Open Settings', onPress: () => Linking.openSettings() },
            ]
          );
        }
      });
    }
  }, [hasPermission]);

  const handleReset = () => {
    scannedRef.current = false;
    setIsActive(true);
  };

  if (!hasPermission) {
    return (
      <View style={nativeStyles.centered}>
        <Text style={nativeStyles.message}>Camera permission is required to scan QR codes.</Text>
        <TouchableOpacity style={nativeStyles.button} onPress={() => requestPermission()}>
          <Text style={nativeStyles.buttonText}>Grant Permission</Text>
        </TouchableOpacity>
      </View>
    );
  }

  return (
    <View style={nativeStyles.container}>
      <CodeScanner
        style={StyleSheet.absoluteFill}
        isActive={isActive}
        barcodeFormats={['qr-code']}
        onBarcodeScanned={(codes) => {
          if (scannedRef.current) return;
          const first = codes[0];
          console.log(first);
          if (first?.rawValue) {
            scannedRef.current = true;
            setIsActive(false);
            onScan(first.rawValue);
          }
        }}
        onError={(error) => {
          console.error('QR scan error:', error);
        }}
      />

      <Overlay 
        isActive={isActive} 
        handleReset={handleReset} 
        onClose={onClose} 
      />
    </View>
  );
}

function Overlay({ isActive, handleReset, onClose }: { isActive: boolean; handleReset: () => void; onClose?: () => void }) {
  return (
    <View style={nativeStyles.overlay} pointerEvents="box-none">
      <View style={nativeStyles.topBar} />
      <View style={nativeStyles.middleRow}>
        <View style={nativeStyles.sideBar} />
        <View style={nativeStyles.scanWindow}>
          <View style={[nativeStyles.corner, nativeStyles.topLeft]} />
          <View style={[nativeStyles.corner, nativeStyles.topRight]} />
          <View style={[nativeStyles.corner, nativeStyles.bottomLeft]} />
          <View style={[nativeStyles.corner, nativeStyles.bottomRight]} />
        </View>
        <View style={nativeStyles.sideBar} />
      </View>
      <View style={nativeStyles.bottomBar}>
        <Text style={nativeStyles.hint}>
          {!isActive ? 'QR code scanned!' : 'Align QR code within the frame'}
        </Text>
        {!isActive && (
          <TouchableOpacity style={nativeStyles.button} onPress={handleReset}>
            <Text style={nativeStyles.buttonText}>Scan Again</Text>
          </TouchableOpacity>
        )}
        {onClose && (
          <TouchableOpacity style={nativeStyles.closeButton} onPress={onClose}>
            <Text style={nativeStyles.closeText}>Close</Text>
          </TouchableOpacity>
        )}
      </View>
    </View>
  )
}