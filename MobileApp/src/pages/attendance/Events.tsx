import { useState } from 'react';
import { View, Text, Platform, ScrollView, ActivityIndicator } from 'react-native';
import { webStyles } from '../../styles/attendanceStyle';
import WebQRScanner from '../../components/QRCodeScanner.web';
import NativeQRScanner from '../../components/QRCodeScanner.native';
import colors from '../../enums/colors';

export default function EventsAttendance() {
  const [qrResult, setQrResult] = useState<string>('');
  const [error, setError] = useState<string>('');
  const [loading, setLoading] = useState(false);

  const handleScan = (result: any) => {
    if (!result) return;
    
    // TODO: if result is not from our system, show error message

    setLoading(true);
    try {
      // TODO: Send API request
    } catch (err) {
      setError('Failed to process QR code. Please try again.');
    } finally {
      setTimeout(() => {
        setLoading(false);
      }, 5000);
      // setLoading(false);
    }

    // TODO: If result is Ok, green checkmark, else red cross with error message

    setQrResult(result || '');
  }

  if(loading) {
    return (
      <View style={webStyles.centered}>
        <ActivityIndicator size="large" color={colors.DEFAULT_BUTTON_COLOR} />
        <Text style={webStyles.hint}>Processing attendance...</Text>
      </View>
    );
  }

  return (
    <ScrollView style={webStyles.container}> 
      {Platform.OS === 'web' ? (
        <WebQRScanner onScan={handleScan} />
      ) : (
        <NativeQRScanner onScan={handleScan} />
      )}

      <View style={{ width: '100%', height: 2, backgroundColor: '#b1b1b1', marginVertical: 16 }} />

      <View style={{ backgroundColor: '#f0f0f0' }}>
        <View>
          <Text style={{ textAlign: 'center', paddingVertical: 16 }}>{qrResult || 'No QR code scanned yet.'}</Text>
        </View>
      </View>
    </ScrollView>
  )
};