import { useState } from 'react';
import { View, Text, Platform, ScrollView, ActivityIndicator } from 'react-native';
import attendanceStyle from '../../styles/attendanceStyle';
import WebQRScanner from '../../components/QRCodeScanner.web';
import NativeQRScanner from '../../components/QRCodeScanner.native';
import colors from '../../enums/colors';

export default function SessionAttendance() {
  const [qrResult, setQrResult] = useState<string>('');
  const [error, setError] = useState<string>('');
  const [loading, setLoading] = useState(false);

  const handleScan = (result: string) => {
    if (!result) return;
    
    if (!result.startsWith('io-event-connecter://')) {
      setError('Invalid QR code format.');
      return;
    }
    // TODO: if result is not from our system, show error message

    setLoading(true);

    try {
      // TODO: Send API request
    } catch (err) {
      setError('Failed to process QR code. Please try again.');
    } finally {
      setTimeout(() => {
        setLoading(false);
      }, 1000); // Simulate processing time
      // setLoading(false);
    }

    // TODO: If result is Ok, green checkmark, else red cross with error message

    setQrResult(result || '');
  }

  if(loading) {
    return (
      <View style={attendanceStyle.centered}>
        <ActivityIndicator size="large" color={colors.DEFAULT_BUTTON_COLOR} />
        <Text style={attendanceStyle.hint}>Processing attendance...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={attendanceStyle.container}> 
      {Platform.OS === 'web' ? (
        <WebQRScanner onScan={handleScan} />
      ) : (
        <NativeQRScanner onScan={handleScan} />
      )}

      <View style={{ width: '100%', height: 2, backgroundColor: '#b1b1b1', marginVertical: 16 }} />

      <View style={{ backgroundColor: '#f0f0f0' }}>
        <View>
          {error ? <Text style={{ textAlign: 'center', paddingVertical: 16, color: 'red' }}>{error}</Text> : null}
          <Text style={{ textAlign: 'center', paddingVertical: 16 }}>{qrResult || 'No QR code scanned yet.'}</Text>
        </View>
      </View>
    </ScrollView>
  )
};