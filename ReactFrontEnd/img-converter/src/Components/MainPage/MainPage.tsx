import React, { useEffect, useState } from 'react';
import { 
  Container, Title, Text, Button, Group, Table, ActionIcon, 
  Loader, Stack, Paper, Center, Modal, Badge, CloseButton, Checkbox 
} from '@mantine/core';
import { Dropzone } from '@mantine/dropzone';
import { Box } from '@mantine/core';
import { Notifications } from '@mantine/notifications';
import { 
  IconUpload, IconPhoto, IconX, IconDownload, IconTrash, IconFile, IconLogout, IconPlayerPlay 
} from '@tabler/icons-react';

// Import custom services and models
import ImageService from '../API/ImageService';
import { UserImage } from '../../Models/UserImage';
import { useAuth } from '../User/AuthContext';

const MainPage: React.FC = () => {
  const { user, logout } = useAuth();
  
  const [history, setHistory] = useState<UserImage[]>([]);
  const [uploading, setUploading] = useState(false);
  const [loadingHistory, setLoadingHistory] = useState(true);
  const [pendingFiles, setPendingFiles] = useState<File[]>([]);

  // NEW: State for multi-select deletion
  const [selectedImageIds, setSelectedImageIds] = useState<string[]>([]);
  
  const [deleteModalOpen, setDeleteModalOpen] = useState(false);
  const [deleteAllModalOpen, setDeleteAllModalOpen] = useState(false);

  const fetchHistory = async () => {
    try {
      const data = await ImageService.getHistory();
      setHistory(data);
      // Clear selections after fetching new data
      setSelectedImageIds([]);
    } catch (error) {
      console.error("Failed to load history", error);
    } finally {
      setLoadingHistory(false);
    }
  };

  useEffect(() => {
    fetchHistory();
  }, []);

  const handleDrop = (files: File[]) => {
    setPendingFiles((prev) => [...prev, ...files]);
  };

  const removePendingFile = (index: number) => {
    setPendingFiles((prev) => prev.filter((_, i) => i !== index));
  };

  const handleStartConversion = async () => {
    if (pendingFiles.length === 0) return;

    setUploading(true);
    let successCount = 0;
    const errors: string[] = [];

    try {
      const batchSize = 3;
      for (let i = 0; i < pendingFiles.length; i += batchSize) {
        const batch = pendingFiles.slice(i, i + batchSize);
        
        const results = await Promise.allSettled(
          batch.map(async (file) => {
            console.log(`%c Attempting to convert: ${file.name}`, 'color: blue; font-weight: bold');
            try {
              await ImageService.convertImage(file);
              return { success: true, fileName: file.name };
            } catch (fileError: any) {
              console.error(`FAILED to convert: ${file.name}`);
              
              if (fileError.response) {
                console.group('Backend Error Details');
                console.log('Status Code:', fileError.response.status);
                console.log('Error Data:', fileError.response.data);
                console.log('Headers:', fileError.response.headers);
                console.groupEnd();
              }
              
              throw fileError;
            }
          })
        );

        results.forEach((result, index) => {
          if (result.status === 'fulfilled') {
            successCount++;
          } else {
            errors.push(batch[index].name);
          }
        });
      }

      if (errors.length === 0) {
        Notifications.show({
          title: 'Conversion Complete',
          message: `Successfully converted ${successCount} images.`,
          color: 'green',
        });
      } else {
        Notifications.show({
          title: 'Partial Success',
          message: `Converted ${successCount} of ${pendingFiles.length} images. ${errors.length} failed.`,
          color: 'yellow',
        });
      }

      setPendingFiles([]);
      await fetchHistory();

    } catch (error: any) {
      Notifications.show({
        title: 'Error during conversion',
        message: `Successfully converted ${successCount} images. Some files failed.`,
        color: errors.length === pendingFiles.length ? 'red' : 'yellow',
      });
    } finally {
      setUploading(false);
    }
  };

  const handleDownload = async (img: UserImage) => {
    try {
      await ImageService.downloadImage(img.id, `${img.originalFileName}.png`);
    } catch (error) {
      Notifications.show({ title: 'Download Error', color: 'red', message: 'Error' });
    }
  };

  // NEW: Toggle selection of a single image
  const toggleImageSelection = (id: string) => {
    setSelectedImageIds((prev) =>
      prev.includes(id) ? prev.filter((imgId) => imgId !== id) : [...prev, id]
    );
  };

  // NEW: Toggle select all
  const toggleSelectAll = () => {
    if (selectedImageIds.length === history.length) {
      setSelectedImageIds([]);
    } else {
      setSelectedImageIds(history.map((img) => img.id));
    }
  };

  // NEW: Delete selected images
  const handleDeleteSelected = async () => {
    if (selectedImageIds.length === 0) return;

    try {
      await ImageService.deleteImages(selectedImageIds);
      Notifications.show({
        title: 'Success',
        message: `Deleted ${selectedImageIds.length} image(s)`,
        color: 'green',
      });
      await fetchHistory();
    } catch (error) {
      Notifications.show({
        title: 'Delete Error',
        message: 'Failed to delete images',
        color: 'red',
      });
    } finally {
      setDeleteModalOpen(false);
    }
  };

  // NEW: Delete all images
  const handleDeleteAll = async () => {
    try {
      await ImageService.deleteAll();
      Notifications.show({
        title: 'Success',
        message: 'All images deleted',
        color: 'green',
      });
      await fetchHistory();
    } catch (error) {
      Notifications.show({
        title: 'Delete Error',
        message: 'Failed to delete all images',
        color: 'red',
      });
    } finally {
      setDeleteAllModalOpen(false);
    }
  };

  const formatBytes = (bytes: number) => {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  };

  // Calculate size difference percentage
const calculateSizeDifference = (originalSize: number, convertedSize: number) => {
  const difference = ((convertedSize - originalSize) / originalSize) * 100;
  return difference;
};

// Format the size comparison display
const formatSizeComparison = (img: UserImage) => {
  const difference = calculateSizeDifference(img.sizeInBytes, img.convertedSizeInBytes);
  const isSmaller = difference < 0;
  const absPercentage = Math.abs(difference).toFixed(1);
  
  return {
    color: isSmaller ? 'green' : 'red',
    text: `${isSmaller ? '↓' : '↑'} ${absPercentage}%`,
    isSmaller
  };
};

  return (
    <Container size="md" py="xl">
      <Stack gap="lg">
        {/* HEADER SECTION WITH LOGOUT */}
        <Group justify="space-between" align="center">
          <div>
            <Title order={2}>WebP to PNG Converter</Title>
            <Text c="dimmed">Logged in as: <b>{user?.username}</b></Text>
          </div>
          <Group>
            <Button 
              variant="light" 
              color="red" 
              onClick={() => setDeleteAllModalOpen(true)} 
              disabled={history.length === 0}
              leftSection={<IconTrash size={16} />}
            >
              Clear History
            </Button>
            <Button 
              variant="outline" 
              color="gray" 
              onClick={logout}
              leftSection={<IconLogout size={16} />}
            >
              Logout
            </Button>
          </Group>
        </Group>

        {/* DROPZONE AREA */}
        <Paper withBorder p={0} radius="md" style={{ overflow: 'hidden' }}>
          <Dropzone
            onDrop={handleDrop}
            maxSize={5 * 1024 * 1024}
            accept={['image/webp']}
            loading={uploading}
          >
            <Group justify="center" gap="xl" style={{ minHeight: 150, pointerEvents: 'none' }}>
              <Dropzone.Idle>
                <IconPhoto size={50} stroke={1.5} />
              </Dropzone.Idle>
              <div>
                <Text size="xl" inline>Drag WebP files here</Text>
                <Text size="sm" c="dimmed" inline mt={7}>Files will be added to the queue below</Text>
              </div>
            </Group>
          </Dropzone>

          {pendingFiles.length > 0 && (
            <Box p="md" style={{ borderTop: '1px solid var(--mantine-color-gray-3)' }}>
              <Text size="sm" fw={700} mb="xs">Queue ({pendingFiles.length} files):</Text>
              <Stack gap="xs">
                {pendingFiles.map((file, index) => (
                  <Group key={index} justify="space-between" bg="gray.0" p="xs" style={{ borderRadius: '4px' }}>
                    <Group gap="sm">
                      <IconFile size={16} />
                      <Text size="sm">{file.name}</Text>
                      <Badge variant="outline" size="xs">{formatBytes(file.size)}</Badge>
                    </Group>
                    <CloseButton size="sm" onClick={() => removePendingFile(index)} />
                  </Group>
                ))}
              </Stack>
              
              <Button 
                fullWidth 
                mt="md" 
                color="green" 
                leftSection={<IconPlayerPlay size={16} />}
                onClick={handleStartConversion}
                loading={uploading}
              >
                Convert {pendingFiles.length} {pendingFiles.length === 1 ? 'File' : 'Files'} to PNG
              </Button>
            </Box>
          )}
        </Paper>

        {/* HISTORY SECTION WITH MULTI-SELECT */}
        <Group justify="space-between" align="center" mt="md">
          <Title order={4}>Recent Conversions</Title>
          {selectedImageIds.length > 0 && (
            <Button 
              size="sm"
              color="red" 
              variant="light"
              leftSection={<IconTrash size={16} />}
              onClick={() => setDeleteModalOpen(true)}
            >
              Delete {selectedImageIds.length} Selected
            </Button>
          )}
        </Group>

        {loadingHistory ? (
          <Center py="xl"><Loader /></Center>
        ) : history.length === 0 ? (
          <Paper withBorder radius="md" p="xl">
            <Center>
              <Text c="dimmed">No conversions yet. Upload some WebP files to get started!</Text>
            </Center>
          </Paper>
        ) : (
          <Paper withBorder radius="md">
            <Table striped highlightOnHover>
              <Table.Thead>
                <Table.Tr>
                  <Table.Th style={{ width: '40px' }}>
                    <Checkbox
                      checked={selectedImageIds.length === history.length && history.length > 0}
                      indeterminate={selectedImageIds.length > 0 && selectedImageIds.length < history.length}
                      onChange={toggleSelectAll}
                    />
                  </Table.Th>
                  <Table.Th>File Name</Table.Th>
                  <Table.Th>Original Size (WebP)</Table.Th>
                  <Table.Th>PNG Size</Table.Th>
                  <Table.Th>Change</Table.Th>
                  <Table.Th>Date</Table.Th>
                  <Table.Th>Actions</Table.Th>
                </Table.Tr>
              </Table.Thead>
              <Table.Tbody>
                {history.map((img) => {
                  const sizeComparison = formatSizeComparison(img);
                  return (
                    <Table.Tr key={img.id}>
                      <Table.Td>
                        <Checkbox
                          checked={selectedImageIds.includes(img.id)}
                          onChange={() => toggleImageSelection(img.id)}
                        />
                      </Table.Td>
                      <Table.Td>{img.originalFileName}</Table.Td>
                      <Table.Td>
                        <Text size="sm">{formatBytes(img.sizeInBytes)}</Text>
                      </Table.Td>
                      <Table.Td>
                        <Text size="sm">{formatBytes(img.convertedSizeInBytes)}</Text>
                      </Table.Td>
                      <Table.Td>
                        <Badge 
                          color={sizeComparison.color} 
                          variant="light"
                          size="sm"
                        >
                          {sizeComparison.text}
                        </Badge>
                      </Table.Td>
                      <Table.Td>{new Date(img.createdAt).toLocaleDateString()}</Table.Td>
                      <Table.Td>
                        <Group gap="xs">
                          <ActionIcon color="blue" onClick={() => handleDownload(img)}>
                            <IconDownload size={18} />
                          </ActionIcon>
                          <ActionIcon 
                            color="red" 
                            onClick={() => {
                              setSelectedImageIds([img.id]);
                              setDeleteModalOpen(true);
                            }}
                          >
                            <IconTrash size={18} />
                          </ActionIcon>
                        </Group>
                      </Table.Td>
                    </Table.Tr>
                  );
                })}
              </Table.Tbody>
            </Table>
          </Paper>
        )}
      </Stack>

      {/* DELETE SELECTED MODAL */}
      <Modal 
        opened={deleteModalOpen} 
        onClose={() => setDeleteModalOpen(false)} 
        title="Confirm Deletion" 
        centered
      >
        <Text size="sm">
          Are you sure you want to delete {selectedImageIds.length} image{selectedImageIds.length !== 1 ? 's' : ''}?
        </Text>
        <Group justify="flex-end" mt="md">
          <Button variant="default" onClick={() => setDeleteModalOpen(false)}>Cancel</Button>
          <Button color="red" onClick={handleDeleteSelected}>Delete</Button>
        </Group>
      </Modal>

      {/* DELETE ALL MODAL */}
      <Modal 
        opened={deleteAllModalOpen} 
        onClose={() => setDeleteAllModalOpen(false)} 
        title="Clear All History" 
        centered
      >
        <Text size="sm">
          Are you sure you want to delete all {history.length} images? This action cannot be undone.
        </Text>
        <Group justify="flex-end" mt="md">
          <Button variant="default" onClick={() => setDeleteAllModalOpen(false)}>Cancel</Button>
          <Button color="red" onClick={handleDeleteAll}>Delete All</Button>
        </Group>
      </Modal>
    </Container>
  );
};

export default MainPage;