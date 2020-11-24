class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  version "0.2.59"
  url "https://github.com/Azure/bicep/releases/download/v0.2.59/bicep-osx-x64"
  sha256 "9b07831f1ac59a613ed8f95f23e691387ad2f3f0d70872b0b4737c4736c09013"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
