// Ekipman Silmek için onaylama modalı

let ekipmanSilinecekId = null;          //silinecek ekipman id
window.showEkipmanSilmeModal = function(id) {
    ekipmanSilinecekId = id;
    document.getElementById('ekipmanSilmeModal').style.display = 'flex';
};

window.showEkipmanEkleModal = function() {      //ekleme modalını göster
    document.getElementById('ekipmanEkleModal').style.display = 'flex';
    document.getElementById('ekipmanEkleHata').style.display = 'none';
};

window.onload = function() {
    // Ekipman silme modal işlemleri
    document.getElementById('ekipmanIptalBtn').onclick = function() {
        document.getElementById('ekipmanSilmeModal').style.display = 'none';
        ekipmanSilinecekId = null;
    };
    document.getElementById('ekipmanOnayBtn').onclick = function() {
        if (ekipmanSilinecekId) {
            fetch('/Admin/SilEkipman', {        // SilEkipman için POST isteği gönderecek.
                method: 'POST',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',    //form verisi için.
                    'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value   //güvenlik için
                },
                body: 'id=' + encodeURIComponent(ekipmanSilinecekId)
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    // Tabloyu güncelleyecek.
                    fetch('/Admin/EkipmanStok')
                        .then(resp => resp.text())
                        .then(html => {
                            const parser = new DOMParser();
                            const doc = parser.parseFromString(html, 'text/html');
                            const newTable = doc.querySelector('#ekipmanTabloContainer');
                            if (newTable) {
                                document.getElementById('ekipmanTabloContainer').innerHTML = newTable.innerHTML;
                            }
                        });
                    document.getElementById('ekipmanSilmeModal').style.display = 'none';
                    ekipmanSilinecekId = null;
                } else {
                    alert(data.message || 'Silme işlemi başarısız.');
                }
            });
        }
    };



    
    // Ekipman ekleme modal işlemleri
    document.getElementById('ekipmanEkleIptalBtn').onclick = function() {
        document.getElementById('ekipmanEkleModal').style.display = 'none';
    };

    document.getElementById('ekipmanEkleForm').onsubmit = function(e) {
        e.preventDefault();
        var form = e.target;
        var data = {
            EkipmanAdi: form.EkipmanAdi.value,
            StokAdedi: form.StokAdedi.value
        };

        fetch('/Admin/YeniStok', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
            },
            body: JSON.stringify(data)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Tabloyu güncelle
                fetch('/Admin/EkipmanStok')
                    .then(resp => resp.text())
                    .then(html => {
                        const parser = new DOMParser();
                        const doc = parser.parseFromString(html, 'text/html');
                        const newTable = doc.querySelector('#ekipmanTabloContainer');
                        if (newTable) {
                            document.getElementById('ekipmanTabloContainer').innerHTML = newTable.innerHTML;
                        }
                    });
                
                // Modal'ı kapat
                document.getElementById('ekipmanEkleModal').style.display = 'none';
                
                // Form'u temizle
                form.reset();
                
            } else {
                var hataDiv = document.getElementById('ekipmanEkleHata');
                hataDiv.textContent = "Kayıt başarısız: " + (data.message || 'Bilinmeyen hata');
                hataDiv.style.display = 'block';
            }
        })
        .catch(error => {
            var hataDiv = document.getElementById('ekipmanEkleHata');
            hataDiv.textContent = "Kayıt başarısız: " + error.message;
            hataDiv.style.display = 'block';
        });
    };
};