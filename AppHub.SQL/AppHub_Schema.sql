-- =====================================================
-- AppHub PostgreSQL Veritabanı Schema Script
-- =====================================================
-- Bu script AppHub veritabanı için tüm tabloları oluşturur
-- PostgreSQL 12+ uyumludur
-- =====================================================

-- Veritabanı oluşturma (eğer yoksa)
-- CREATE DATABASE AppHub;

-- Veritabanına bağlan
-- \c AppHub;

-- =====================================================
-- EXTENSIONS
-- =====================================================
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- =====================================================
-- TABLES
-- =====================================================

-- -----------------------------------------------------
-- 1. OAuth Providers Tablosu
-- -----------------------------------------------------
CREATE TABLE oauth_providers (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- OAuth sağlayıcılarını ekle
INSERT INTO oauth_providers (name) VALUES 
    ('Google'),
    ('Facebook'),
    ('Twitter');

-- -----------------------------------------------------
-- 2. Users Tablosu
-- -----------------------------------------------------
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(100) NOT NULL UNIQUE,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255), -- NULL olabilir çünkü OAuth kullanıcıları şifre olmayabilir
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    is_active BOOLEAN DEFAULT TRUE,
    is_email_verified BOOLEAN DEFAULT FALSE,
    last_login TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT email_format CHECK (email ~* '^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$')
);

-- -----------------------------------------------------
-- 3. User OAuth Bağlantıları Tablosu
-- -----------------------------------------------------
CREATE TABLE user_oauth (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    provider_id INTEGER NOT NULL REFERENCES oauth_providers(id) ON DELETE CASCADE,
    provider_user_id VARCHAR(255) NOT NULL, -- OAuth sağlayıcısındaki kullanıcı ID'si
    access_token TEXT, -- Şifrelenmiş token
    refresh_token TEXT, -- Şifrelenmiş refresh token
    token_expires_at TIMESTAMP WITH TIME ZONE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(provider_id, provider_user_id), -- Aynı provider'da aynı kullanıcı ID'si olamaz
    UNIQUE(user_id, provider_id) -- Bir kullanıcı aynı provider'ı sadece bir kez bağlayabilir
);

-- -----------------------------------------------------
-- 4. Applications Tablosu
-- -----------------------------------------------------
CREATE TABLE applications (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL UNIQUE,
    description TEXT,
    app_key VARCHAR(100) UNIQUE, -- Uygulama için benzersiz anahtar
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- -----------------------------------------------------
-- 5. Features Tablosu (Uygulama Özellikleri)
-- -----------------------------------------------------
CREATE TABLE features (
    id SERIAL PRIMARY KEY,
    application_id INTEGER NOT NULL REFERENCES applications(id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    feature_key VARCHAR(100) NOT NULL, -- Özellik için benzersiz anahtar
    is_premium BOOLEAN DEFAULT FALSE, -- Premium özellik mi?
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(application_id, feature_key) -- Aynı uygulamada aynı feature_key olamaz
);

-- -----------------------------------------------------
-- 6. Subscriptions Tablosu (Aylık Üyelikler)
-- -----------------------------------------------------
CREATE TABLE subscriptions (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    application_id INTEGER NOT NULL REFERENCES applications(id) ON DELETE CASCADE,
    start_date DATE NOT NULL,
    end_date DATE NOT NULL,
    status VARCHAR(20) NOT NULL DEFAULT 'active', -- active, expired, cancelled, pending
    purchase_date TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    transaction_id VARCHAR(255), -- İşlem ID'si (in-app purchase)
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT valid_date_range CHECK (end_date >= start_date),
    CONSTRAINT valid_status CHECK (status IN ('active', 'expired', 'cancelled', 'pending'))
);

-- -----------------------------------------------------
-- 7. Subscription Features Tablosu
-- -----------------------------------------------------
-- Kullanıcının aboneliği ile hangi özelliklere erişebileceğini belirler
CREATE TABLE subscription_features (
    id SERIAL PRIMARY KEY,
    subscription_id INTEGER NOT NULL REFERENCES subscriptions(id) ON DELETE CASCADE,
    feature_id INTEGER NOT NULL REFERENCES features(id) ON DELETE CASCADE,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(subscription_id, feature_id) -- Aynı abonelikte aynı özellik tekrar eklenemez
);

-- -----------------------------------------------------
-- 8. Images Tablosu (S3'e yüklenen resimler)
-- -----------------------------------------------------
CREATE TABLE images (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    application_id INTEGER NOT NULL REFERENCES applications(id) ON DELETE CASCADE,
    s3_path VARCHAR(500) NOT NULL, -- S3'teki tam yol
    s3_bucket VARCHAR(255), -- S3 bucket adı
    s3_key VARCHAR(500), -- S3 key (dosya yolu)
    original_filename VARCHAR(255),
    file_size BIGINT, -- Byte cinsinden dosya boyutu
    mime_type VARCHAR(100),
    upload_status VARCHAR(20) DEFAULT 'completed', -- pending, completed, failed
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT valid_upload_status CHECK (upload_status IN ('pending', 'completed', 'failed'))
);

-- -----------------------------------------------------
-- 9. Analysis Results Tablosu
-- -----------------------------------------------------
CREATE TABLE analysis_results (
    id SERIAL PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    application_id INTEGER NOT NULL REFERENCES applications(id) ON DELETE CASCADE,
    image_id INTEGER REFERENCES images(id) ON DELETE SET NULL, -- NULL olabilir, bazı analizler resim gerektirmeyebilir
    result_data JSONB NOT NULL, -- Analiz sonuçları JSON formatında
    analysis_type VARCHAR(100), -- Analiz tipi (örn: 'image_classification', 'object_detection')
    status VARCHAR(20) DEFAULT 'completed', -- pending, processing, completed, failed
    processing_time_ms INTEGER, -- İşlem süresi (milisaniye)
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT valid_analysis_status CHECK (status IN ('pending', 'processing', 'completed', 'failed'))
);

-- -----------------------------------------------------
-- 10. Application Settings Tablosu
-- -----------------------------------------------------
CREATE TABLE application_settings (
    id SERIAL PRIMARY KEY,
    application_id INTEGER NOT NULL REFERENCES applications(id) ON DELETE CASCADE,
    setting_key VARCHAR(255) NOT NULL,
    setting_value JSONB NOT NULL, -- Ayarlar JSON formatında
    description TEXT,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(application_id, setting_key) -- Aynı uygulamada aynı setting_key olamaz
);

-- =====================================================
-- INDEXES
-- =====================================================

-- Users indexes
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_username ON users(username);
CREATE INDEX idx_users_created_at ON users(created_at);

-- User OAuth indexes
CREATE INDEX idx_user_oauth_user_id ON user_oauth(user_id);
CREATE INDEX idx_user_oauth_provider_id ON user_oauth(provider_id);
CREATE INDEX idx_user_oauth_provider_user_id ON user_oauth(provider_user_id);

-- Applications indexes
CREATE INDEX idx_applications_name ON applications(name);
CREATE INDEX idx_applications_app_key ON applications(app_key);
CREATE INDEX idx_applications_is_active ON applications(is_active);

-- Features indexes
CREATE INDEX idx_features_application_id ON features(application_id);
CREATE INDEX idx_features_feature_key ON features(feature_key);
CREATE INDEX idx_features_is_premium ON features(is_premium);

-- Subscriptions indexes
CREATE INDEX idx_subscriptions_user_id ON subscriptions(user_id);
CREATE INDEX idx_subscriptions_application_id ON subscriptions(application_id);
CREATE INDEX idx_subscriptions_status ON subscriptions(status);
CREATE INDEX idx_subscriptions_dates ON subscriptions(start_date, end_date);
CREATE INDEX idx_subscriptions_active ON subscriptions(user_id, application_id, status) WHERE status = 'active';

-- Subscription Features indexes
CREATE INDEX idx_subscription_features_subscription_id ON subscription_features(subscription_id);
CREATE INDEX idx_subscription_features_feature_id ON subscription_features(feature_id);

-- Images indexes
CREATE INDEX idx_images_user_id ON images(user_id);
CREATE INDEX idx_images_application_id ON images(application_id);
CREATE INDEX idx_images_created_at ON images(created_at);
CREATE INDEX idx_images_s3_path ON images(s3_path);

-- Analysis Results indexes
CREATE INDEX idx_analysis_results_user_id ON analysis_results(user_id);
CREATE INDEX idx_analysis_results_application_id ON analysis_results(application_id);
CREATE INDEX idx_analysis_results_image_id ON analysis_results(image_id);
CREATE INDEX idx_analysis_results_created_at ON analysis_results(created_at);
CREATE INDEX idx_analysis_results_status ON analysis_results(status);
CREATE INDEX idx_analysis_results_type ON analysis_results(analysis_type);
-- JSONB index for faster queries on result_data
CREATE INDEX idx_analysis_results_result_data ON analysis_results USING GIN (result_data);

-- Application Settings indexes
CREATE INDEX idx_application_settings_application_id ON application_settings(application_id);
CREATE INDEX idx_application_settings_key ON application_settings(setting_key);
-- JSONB index for faster queries on setting_value
CREATE INDEX idx_application_settings_value ON application_settings USING GIN (setting_value);

-- =====================================================
-- FUNCTIONS & TRIGGERS
-- =====================================================

-- Updated_at otomatik güncelleme fonksiyonu
CREATE OR REPLACE FUNCTION update_updated_at_column()
RETURNS TRIGGER AS $$
BEGIN
    NEW.updated_at = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$ language 'plpgsql';

-- Updated_at trigger'ları
CREATE TRIGGER update_users_updated_at BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_user_oauth_updated_at BEFORE UPDATE ON user_oauth
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_applications_updated_at BEFORE UPDATE ON applications
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_subscriptions_updated_at BEFORE UPDATE ON subscriptions
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_analysis_results_updated_at BEFORE UPDATE ON analysis_results
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

CREATE TRIGGER update_application_settings_updated_at BEFORE UPDATE ON application_settings
    FOR EACH ROW EXECUTE FUNCTION update_updated_at_column();

-- Abonelik durumunu otomatik kontrol eden fonksiyon
CREATE OR REPLACE FUNCTION check_subscription_status()
RETURNS TRIGGER AS $$
BEGIN
    -- Eğer end_date geçmişse ve status active ise, expired olarak işaretle
    IF NEW.end_date < CURRENT_DATE AND NEW.status = 'active' THEN
        NEW.status = 'expired';
    END IF;
    RETURN NEW;
END;
$$ language 'plpgsql';

CREATE TRIGGER check_subscription_status_trigger BEFORE INSERT OR UPDATE ON subscriptions
    FOR EACH ROW EXECUTE FUNCTION check_subscription_status();

-- =====================================================
-- VIEWS (İsteğe Bağlı - Kolay Sorgular İçin)
-- =====================================================

-- Aktif abonelikleri gösteren view
CREATE OR REPLACE VIEW active_subscriptions AS
SELECT 
    s.id,
    s.user_id,
    u.username,
    u.email,
    s.application_id,
    a.name AS application_name,
    s.start_date,
    s.end_date,
    s.status,
    s.purchase_date
FROM subscriptions s
JOIN users u ON s.user_id = u.id
JOIN applications a ON s.application_id = a.id
WHERE s.status = 'active' AND s.end_date >= CURRENT_DATE;

-- Kullanıcının erişebileceği özellikleri gösteren view
CREATE OR REPLACE VIEW user_accessible_features AS
SELECT DISTINCT
    u.id AS user_id,
    u.username,
    a.id AS application_id,
    a.name AS application_name,
    f.id AS feature_id,
    f.name AS feature_name,
    f.feature_key,
    s.id AS subscription_id,
    s.status AS subscription_status
FROM users u
JOIN subscriptions s ON u.id = s.user_id
JOIN applications a ON s.application_id = a.id
JOIN subscription_features sf ON s.id = sf.subscription_id
JOIN features f ON sf.feature_id = f.id
WHERE s.status = 'active' AND s.end_date >= CURRENT_DATE;

-- Analiz sonuçlarını detaylı gösteren view
CREATE OR REPLACE VIEW analysis_results_detail AS
SELECT 
    ar.id,
    ar.user_id,
    u.username,
    ar.application_id,
    a.name AS application_name,
    ar.image_id,
    i.original_filename,
    i.s3_path,
    ar.analysis_type,
    ar.result_data,
    ar.status,
    ar.processing_time_ms,
    ar.created_at
FROM analysis_results ar
JOIN users u ON ar.user_id = u.id
JOIN applications a ON ar.application_id = a.id
LEFT JOIN images i ON ar.image_id = i.id;

-- =====================================================
-- COMMENTS (Tablo ve Kolon Açıklamaları)
-- =====================================================

COMMENT ON TABLE users IS 'Sistem kullanıcıları - hem normal hem de OAuth kullanıcıları';
COMMENT ON TABLE oauth_providers IS 'OAuth sağlayıcıları (Google, Facebook, Twitter)';
COMMENT ON TABLE user_oauth IS 'Kullanıcıların OAuth sağlayıcıları ile bağlantıları';
COMMENT ON TABLE applications IS 'Sistemdeki uygulamalar';
COMMENT ON TABLE features IS 'Uygulamaların özellikleri';
COMMENT ON TABLE subscriptions IS 'Kullanıcıların aylık uygulama abonelikleri';
COMMENT ON TABLE subscription_features IS 'Aboneliklerin sahip olduğu özellikler';
COMMENT ON TABLE images IS 'Amazon S3''e yüklenen resimler ve metadata';
COMMENT ON TABLE analysis_results IS 'Uygulama bazlı analiz sonuçları';
COMMENT ON TABLE application_settings IS 'Uygulama bazlı ayarlar';

COMMENT ON COLUMN users.password_hash IS 'NULL olabilir - OAuth kullanıcıları şifre olmayabilir';
COMMENT ON COLUMN images.s3_path IS 'S3''teki tam dosya yolu';
COMMENT ON COLUMN analysis_results.result_data IS 'JSON formatında analiz sonuçları';
COMMENT ON COLUMN application_settings.setting_value IS 'JSON formatında ayar değerleri';

-- =====================================================
-- SAMPLE DATA (İsteğe Bağlı - Test İçin)
-- =====================================================

-- Örnek uygulama ekleme (isteğe bağlı)
-- INSERT INTO applications (name, description, app_key) VALUES 
--     ('Image Analyzer', 'Görüntü analiz uygulaması', 'image_analyzer'),
--     ('Face Recognition', 'Yüz tanıma uygulaması', 'face_recognition');

-- =====================================================
-- END OF SCRIPT
-- =====================================================

